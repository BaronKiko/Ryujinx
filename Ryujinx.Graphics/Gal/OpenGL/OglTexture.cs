using OpenTK.Graphics.OpenGL;
using Ryujinx.Graphics.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using Ryujinx.Common.Logging;

namespace Ryujinx.Graphics.Gal.OpenGL
{
    class OglTexture : IGalTexture
    {
        private const long MaxTextureCacheSize = 768 * 1024 * 1024;

        private OglSizedCachedResource _textureCache;

        public EventHandler<int> TextureDeleted { get; set; }

        public OglTexture()
        {
            _textureCache = new OglSizedCachedResource(DeleteTexture, MaxTextureCacheSize);
        }

        public void LockCache()
        {
            _textureCache.Lock();
        }

        public void UnlockCache()
        {
            _textureCache.Unlock();
        }

        private void DeleteTexture(ImageHandler cachedImage)
        {
            if (cachedImage.Handle.Layer != -1)
            {
                Logger.PrintWarning(LogClass.Gpu, "Deleted texture layer, should this be supported?");
            }

            TextureDeleted?.Invoke(this, cachedImage.Handle.ParentHandle);

            GL.DeleteTexture(cachedImage.Handle.ParentHandle);
        }

        //These framebuffers are used to blit images
        private int _srcFb;
        private int _dstFb;

        private int _dummyTexture;

        public void Copy(
            ImageHandler srcTex,
            ImageHandler dstTex,
            int dstLayer,
            int dstLevel,
            int srcX0,
            int srcY0,
            int srcX1,
            int srcY1,
            int dstX0,
            int dstY0,
            int dstX1,
            int dstY1)
        {
            if (srcTex.HasColor != dstTex.HasColor ||
                srcTex.HasDepth != dstTex.HasDepth ||
                srcTex.HasStencil != dstTex.HasStencil)
            {
                //return;
            }

            if (_srcFb == 0)
            {
                _srcFb = GL.GenFramebuffer();
            }

            if (_dstFb == 0)
            {
                _dstFb = GL.GenFramebuffer();
            }

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _srcFb);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _dstFb);

            FramebufferAttachment srcAttachment = OglRenderTarget.GetAttachment(srcTex);

            if (srcTex.Handle.ViewHandle != -1)
            {
                GL.FramebufferTexture(FramebufferTarget.ReadFramebuffer, srcAttachment, srcTex.Handle.ViewHandle, 0);
            }
            else if (srcTex.Handle.Layer == -1)
            {
                GL.FramebufferTexture(FramebufferTarget.ReadFramebuffer, srcAttachment, srcTex.Handle.ParentHandle, srcTex.Handle.Level);
            }
            else
            {
                GL.FramebufferTextureLayer(FramebufferTarget.ReadFramebuffer, srcAttachment, srcTex.Handle.ParentHandle, srcTex.Handle.Level, srcTex.Handle.Layer);
            }

            FramebufferAttachment dstAttachment = OglRenderTarget.GetAttachment(dstTex);

            if (dstTex.Handle.Layer != -1)
            {
                dstLayer += dstTex.Handle.Layer;
            }

            if (dstTex.Handle.Level != -1)
            {
                dstLayer += dstTex.Handle.Level;
            }

            if (dstLayer > 0)
            {
                GL.FramebufferTextureLayer(FramebufferTarget.DrawFramebuffer, dstAttachment, dstTex.Handle.ParentHandle, dstLevel, dstLayer);
            }
            else
            {
                GL.FramebufferTexture(FramebufferTarget.DrawFramebuffer, dstAttachment, dstTex.Handle.ParentHandle, dstLevel);
            }


            BlitFramebufferFilter filter = BlitFramebufferFilter.Nearest;

            if (srcTex.HasColor)
            {
                GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

                filter = BlitFramebufferFilter.Linear;
            }

            ClearBufferMask mask = OglRenderTarget.GetClearMask(srcTex);

            GL.BlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);

            Logger.PrintInfo(LogClass.Gpu, GL.CheckFramebufferStatus(FramebufferTarget.ReadFramebuffer).ToString());
            Logger.PrintInfo(LogClass.Gpu, GL.CheckFramebufferStatus(FramebufferTarget.DrawFramebuffer).ToString());
        }

        public ImageHandler Create(TextureKey key, byte[][] data, GalImage image)
        {
            int level = 0;
            const int border = 0;

            // Create texture without handle as we don't know it however we will need it to generate a map
            ImageHandler newTexture = new ImageHandler(-1, key, image);

            ImageHandler oldTexture = _textureCache.AddOrUpdate(key, newTexture);

            // Get new texture map, empty if there is only 1 layer
            List<KeyValuePair<int[], ImageHandler>> toMerge = GetToMerge(newTexture, key.Position, key.Size);


            // We can ignore children of old texture as GetToMerge will find them if they exist
            if (oldTexture != null)
            {
                int[] mapIndexes = ImageUtils.GetMapIndex(newTexture.Map, 0, oldTexture.Image.Size, newTexture.Image.Size);
                if (mapIndexes.Length > 0)
                {
                    toMerge.Add(new KeyValuePair<int[], ImageHandler>(mapIndexes, oldTexture));
                }
            }

            // Try get parent texture
            int[] parentMapIndexes = _textureCache.TryGetParentTexture(key.Position, key.Size, out ImageHandler parentHandler);

            // We need to work out what texture handle and target to use
            int handle = newTexture.Handle.ParentHandle;
            TextureTarget target;

            // New texture has parent
            if (parentMapIndexes.Length > 0)
            {
                Logger.PrintWarning(LogClass.Gpu, $"Has Parent {handle}");
                // New texture has both a valid parent and either valid children that need to be merged or data tha needs to be loaded
                // We need to use a dummy texture to store contents ready for further merge to parent.
                if (toMerge.Count > 0 || data != null)
                {
                    //if (!texturePreexisted)
                    {
                        // Only need dummy
                        if (_dummyTexture == 0)
                        {
                            _dummyTexture = GL.GenTexture();
                        }

                        handle = _dummyTexture;
                    }

                    target = ImageUtils.GetTextureTarget(parentHandler.Image.TextureTarget);
                }
                // New texture didn't provide any data to copy, has no children and parent already exists.
                // This means this new texture already exists and because we have just created the handler
                // we can safely return without creating a real texture
                else
                {
                    Logger.PrintWarning(LogClass.Gpu, "Skipped Create");
                    CopyToParent(parentHandler, parentMapIndexes, newTexture, false);
                    return newTexture;
                }
            }
            // Has a no parent so we need to actually generate the texture
            else
            {
                Logger.PrintWarning(LogClass.Gpu, $"No Parent {handle}");

                //if (!texturePreexisted)
                {
                    handle = GL.GenTexture();
                }

                target = ImageUtils.GetTextureTarget(image.TextureTarget);
            }

            // If it's dummy texture it will be replaced later as all dummy textures have a parent
            newTexture.Handle = new TextureHandle(handle);

            // Finally worked out handle, we can bind and generate the texture safely now
            GL.BindTexture(target, handle);

            GL.TexParameter(target, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(target, TextureParameterName.TextureMaxLevel, Math.Max(image.MaxMipmapLevel - 1, 0));

            bool compressed = ImageUtils.IsCompressed(image.Format);

            // TODO: Should compressed textures without data be supported?
            if (data == null && compressed)
            {
                throw new InvalidOperationException("Surfaces with compressed formats and no data are not supported!");
            }

            if (ImageUtils.IsArray(image.TextureTarget))
                GL.TexStorage3D(TextureTarget3d.Texture2DArray, Math.Max(image.MaxMipmapLevel, 1), (SizedInternalFormat)OglEnumConverter.GetImageFormat(image.Format).Item1, image.Width, image.Height, image.LayerCount);
            else
                GL.TexStorage2D(TextureTarget2d.Texture2D, Math.Max(image.MaxMipmapLevel, 1), (SizedInternalFormat)OglEnumConverter.GetImageFormat(image.Format).Item1, image.Width, image.Height);
            
            if (data == null)
            {
                Logger.PrintWarning(LogClass.Gpu,
                    $"No data {target} {key.Position}-{key.Size} {handle} {image.Width}x{image.Height}x{image.Depth}-{image.LayerCount}");
                //if (!texturePreexisted)
                {
                    //CreateTex(target, image.Width, image.Height, image.Depth, level, image , border);
                    GL.GenerateMipmap((GenerateMipmapTarget)target);
                }
            }
            else
            if (data != null)
            {
                for (level = 0; level < image.MaxMipmapLevel; level++)
                {
                    if (level == 1)
                        GL.GenerateMipmap((GenerateMipmapTarget)target);

                    (int, int, int)[] sizes = ImageUtils.GetMipmapDimensions(image);
                    Logger.PrintWarning(LogClass.Gpu,
                        $"Provided data {target} {key.Position}-{key.Size} {handle} {image.Width}x{image.Height}x{image.Depth}-{image.LayerCount}");
                    CreateTexWithData(target, sizes[level].Item1, sizes[level].Item2, sizes[level].Item3, level,
                        image, border, data[level], compressed);

                    if (image.MaxMipmapLevel == 1)
                        GL.GenerateMipmap((GenerateMipmapTarget)target);
                }
            }

            // Next we need to merge children into parent
            if (toMerge.Count() > 0)
            {
                Merge(newTexture, toMerge, (data == null));
            }

            // Finally we need to copy to the parent if present
            if (parentMapIndexes.Length > 0)
            {
                Logger.PrintWarning(LogClass.Gpu, "Copy to Parent");
                CopyToParent(parentHandler, parentMapIndexes, newTexture, true);
            }

            // We can add this immedietly as newTexture will keep our reference for when we find out the handle. This saves us having to add it on all paths later
            //_textureCache.AddOrUpdate(key, newTexture);

            /*if (hasParent)
            {
                int layerStride = ImageUtils.GetLayerStride(parentHandler.Image);
                int layer = GetLayer(parentHandler.Handle.Key, newTexture.Handle.Key, layerStride);
                GL.TextureView(newTexture.Handle.Key, TextureTarget.ProxyTexture2D, parentHandler.Handle.Key, internalFmt, 0, 1, layer, image.LayerCount);
            }*/

            return newTexture;
        }

        private void CreateTex(TextureTarget target, int width, int height, int depth, int level, GalImage image, int border)
        {
            (PixelInternalFormat internalFmt,
             PixelFormat format,
             PixelType type) = OglEnumConverter.GetImageFormat(image.Format);

            switch (target)
            {
                case TextureTarget.Texture1D:
                    GL.TexImage1D(
                        target,
                        level,
                        internalFmt,
                        width,
                        border,
                        format,
                        type,
                        IntPtr.Zero);
                    break;

                case TextureTarget.Texture2D:
                    GL.TexImage2D(
                        target,
                        level,
                        internalFmt,
                        width,
                        height,
                        border,
                        format,
                        type,
                        IntPtr.Zero);
                    break;
                case TextureTarget.Texture3D:
                    GL.TexImage3D(
                        target,
                        level,
                        internalFmt,
                        width,
                        height,
                        depth,
                        border,
                        format,
                        type,
                        IntPtr.Zero);
                    break;
                // Cube map arrays are just 2D texture arrays with 6 entries
                // per cube map so we can handle them in the same way
                case TextureTarget.TextureCubeMapArray:
                case TextureTarget.Texture2DArray:
                    GL.TexImage3D(
                        target,
                        level,
                        internalFmt,
                        width,
                        height,
                        image.LayerCount,
                        border,
                        format,
                        type,
                        IntPtr.Zero);
                    break;

                case TextureTarget.TextureCubeMap:
                    //Note: Each row of the texture needs to be aligned to 4 bytes.
                    int pitch = (width * ImageUtils.GetImageDescriptor(image.Format).BytesPerPixel + 3) & ~3;
                    int faceSize = height * pitch * depth;

                    for (int face = 0; face < 6; face++)
                    {
                        GL.TexImage2D(
                            TextureTarget.TextureCubeMapPositiveX + face,
                            level,
                            internalFmt,
                            width,
                            height,
                            border,
                            format,
                            type,
                            IntPtr.Zero);
                    }
                    break;
                default:
                    throw new NotImplementedException($"Unsupported texture target type: {target}");
            }
        }

        private void CreateTexWithData(TextureTarget target, int width, int height, int depth, int level, GalImage image, int border, byte[] data, bool compressed)
        {
            if (compressed && !IsAstc(image.Format))
            {
                InternalFormat internalFmt = OglEnumConverter.GetCompressedImageFormat(image.Format);

                switch (target)
                {
                    case TextureTarget.Texture1D:
                        GL.CompressedTexSubImage1D(
                            target,
                            level,
                            0, width, format,
                            width,
                            border,
                            data.Length,
                            data);
                        break;
                    case TextureTarget.Texture2D:
                        GL.CompressedTexImage2D(
                            target,
                            level,
                            internalFmt,
                            width,
                            height,
                            border,
                            data.Length,
                            data);
                        break;
                    case TextureTarget.Texture3D:
                        GL.CompressedTexImage3D(
                            target,
                            level,
                            internalFmt,
                            width,
                            height,
                            depth,
                            border,
                            data.Length,
                            data);
                        break;
                    // Cube map arrays are just 2D texture arrays with 6 entries
                    // per cube map so we can handle them in the same way
                    case TextureTarget.TextureCubeMapArray:
                    case TextureTarget.Texture2DArray:
                        GL.CompressedTexImage3D(
                            target,
                            level,
                            internalFmt,
                            width,
                            height,
                            image.LayerCount,
                            border,
                            data.Length,
                            data);
                        break;
                    case TextureTarget.TextureCubeMap:
                        Span<byte> array = new Span<byte>(data);

                        int faceSize = data.Length / 6;

                        for (int Face = 0; Face < 6; Face++)
                        {
                            GL.CompressedTexImage2D(
                                TextureTarget.TextureCubeMapPositiveX + Face,
                                level,
                                internalFmt,
                                width,
                                height,
                                border,
                                faceSize,
                                array.Slice(Face * faceSize, faceSize).ToArray());
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Unsupported texture target type: {target}");
                }
            }
            else
            {
                //TODO: Use KHR_texture_compression_astc_hdr when available
                if (IsAstc(image.Format))
                {
                    int textureBlockWidth = ImageUtils.GetBlockWidth(image.Format);
                    int textureBlockHeight = ImageUtils.GetBlockHeight(image.Format);
                    int textureBlockDepth = ImageUtils.GetBlockDepth(image.Format);

                    data = AstcDecoder.DecodeToRgba8888(
                        data,
                        textureBlockWidth,
                        textureBlockHeight,
                        textureBlockDepth,
                        width,
                        height,
                        depth);

                    image.Format = GalImageFormat.Rgba8 | (image.Format & GalImageFormat.TypeMask);
                }

                (PixelInternalFormat internalFmt,
                 PixelFormat format,
                 PixelType type) = OglEnumConverter.GetImageFormat(image.Format);


                switch (target)
                {
                    case TextureTarget.Texture1D:
                        GL.TexImage1D(
                            target,
                            level,
                            internalFmt,
                            width,
                            border,
                            format,
                            type,
                            data);
                        break;
                    case TextureTarget.Texture2D:
                        GL.TexImage2D(
                            target,
                            level,
                            internalFmt,
                            width,
                            height,
                            border,
                            format,
                            type,
                            data);
                        break;
                    case TextureTarget.Texture3D:
                        GL.TexImage3D(
                            target,
                            level,
                            internalFmt,
                            width,
                            height,
                            depth,
                            border,
                            format,
                            type,
                            data);
                        break;
                    // Cube map arrays are just 2D texture arrays with 6 entries
                    // per cube map so we can handle them in the same way
                    case TextureTarget.TextureCubeMapArray:
                    case TextureTarget.Texture2DArray:
                        GL.TexImage3D(
                            target,
                            level,
                            internalFmt,
                            width,
                            height,
                            image.LayerCount,
                            border,
                            format,
                            type,
                            data);
                        break;
                    case TextureTarget.TextureCubeMap:
                        Span<byte> array = new Span<byte>(data);

                        int faceSize = data.Length / 6;

                        for (int face = 0; face < 6; face++)
                        {
                            GL.TexImage2D(
                                TextureTarget.TextureCubeMapPositiveX + face,
                                level,
                                internalFmt,
                                width,
                                height,
                                border,
                                format,
                                type,
                                array.Slice(face * faceSize, faceSize).ToArray());
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Unsupported texture target type: {target}");
                }
            }
        }

        private List<KeyValuePair<int[], ImageHandler>> GetToMerge(ImageHandler image, long key, long size)
        {
            List<KeyValuePair<int[], ImageHandler>> toMerge = new List<KeyValuePair<int[], ImageHandler>>();
            foreach (var overlap in _textureCache.GetInRange(key, size, image.Image.TextureTarget))
            {
                int[] mapIndexes = ImageUtils.GetMapIndex(image.Map, (overlap.Key.Position - key),
                    overlap.Image.Size, size);
                if (mapIndexes.Length > 0)
                {
                    toMerge.Add(new KeyValuePair<int[], ImageHandler>(mapIndexes, overlap));
                }
            }

            return toMerge;
        }

        private void Merge(ImageHandler destination, List<KeyValuePair<int[], ImageHandler>> sources, bool copySubTextures)
        {
            Logger.PrintWarning(LogClass.Gpu, "Merging");

            List<int> copied = new List<int>();

            foreach (KeyValuePair<int[], ImageHandler> handler in sources)
            {
                if (copied.Contains(handler.Value.Handle.ParentHandle) || handler.Value.Handle.Layer != -1)
                {
                    Logger.PrintWarning(LogClass.Gpu, $"Skiping stage 1 {handler.Value.Handle.ParentHandle}:{handler.Value.Handle.Layer}:{handler.Value.Handle.Level}");
                    continue;
                }

                copied.Add(handler.Value.Handle.ParentHandle);
                CopyToParent(destination, handler.Key, handler.Value, copySubTextures);
            }

            foreach (KeyValuePair<int[], ImageHandler> handler in sources)
            {
                if (copied.Contains(handler.Value.Handle.ParentHandle))
                {
                    Logger.PrintWarning(LogClass.Gpu, $"Skiping stage 2 {handler.Value.Handle.ParentHandle}:{handler.Value.Handle.Layer}:{handler.Value.Handle.Level}");
                    continue;
                }

                copied.Add(handler.Value.Handle.ParentHandle);
                CopyToParent(destination, handler.Key, handler.Value, copySubTextures);
                //handler.Handle = new KeyValuePair<int, int>(GL.GenTexture(), -1);
                //GL.TextureView(handler.Handle.Key, TextureTarget.ProxyTexture2D, destination.Handle.Key, OglEnumConverter.GetImageFormat(handler.Image.Format).Item1, 0, 1, layer, handler.Image.LayerCount);

                //handler.Image.Format = destination.Format;
            }
        }

        private void CopyToParent(ImageHandler parent, int[] mapIndexes, ImageHandler child, bool copySubTextures)
        {
            // Some simple error checks
            if (mapIndexes.Length == 0)
            {
                Logger.PrintWarning(LogClass.Gpu, "Couldn't find source in map, skipping");
                return;
            }

            if (parent.Handle.ParentHandle == child.Handle.ParentHandle)
            {
                Logger.PrintInfo(LogClass.Gpu, "Texture already copied to parent");
                return;
            }

            bool isParentLayered = ImageUtils.IsLayered(parent.TopLevelImage.TextureTarget);
            bool isChildLayered  = ImageUtils.IsLayered(child.TopLevelImage.TextureTarget);

            /*layer = parent.Map[mapIndex].Layer;

            if (layer == -1)
            {
                Logger.PrintWarning(LogClass.Gpu, "Failed to merge layer, skipping");
                return;
            }

            Logger.PrintWarning(LogClass.Gpu, $"Layer {layer}: {mapIndex:X}");*/

            int baseLayer = parent.Map[mapIndexes[0]].Layer;
            int baseLevel = parent.Map[mapIndexes[0]].Level;

            // Copy texture
            if (copySubTextures)
            {
                if (_srcFb == 0)
                {
                    _srcFb = GL.GenFramebuffer();
                }

                if (_dstFb == 0)
                {
                    _dstFb = GL.GenFramebuffer();
                }

                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _srcFb);
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _dstFb);

                FramebufferAttachment childAttachment  = OglRenderTarget.GetAttachment(child);
                FramebufferAttachment parentAttachment = OglRenderTarget.GetAttachment(parent);

                int levelLayer = baseLayer;

                foreach (int mapIndex in mapIndexes)
                {
                    // Calculate child layer
                    int childLayer = parent.Map[mapIndex].Layer - baseLayer;
                    if (child.Handle.Layer != -1)
                    {
                        childLayer += child.Handle.Layer;
                    }

                    if (child.TopLevelImage.LayerCount < childLayer)
                    {
                        Logger.PrintWarning(LogClass.Gpu, "Tried to copy out of bounds layer from child");
                    }

                    // Calculate level
                    // We need to reset base level when we have wrapped around to the next layer
                    if (levelLayer != parent.Map[mapIndex].Layer)
                    {
                        levelLayer = parent.Map[mapIndex].Layer;
                        baseLevel = 0;
                    }

                    int childLevel = parent.Map[mapIndex].Level - baseLevel + child.Handle.Level;

                    if (child.TopLevelImage.MaxMipmapLevel <= childLevel)
                    {
                        Logger.PrintWarning(LogClass.Gpu, "Tried to copy out of bounds level from child");
                    }

                    // Attach child texture
                    if (!isChildLayered)
                    {
                        GL.FramebufferTexture(FramebufferTarget.ReadFramebuffer, childAttachment, child.Handle.ParentHandle, childLevel);
                    }
                    else
                    {
                        GL.FramebufferTextureLayer(FramebufferTarget.ReadFramebuffer, childAttachment, child.Handle.ParentHandle, childLevel, childLayer);
                    }


                    // Calculate parent layer
                    int parentLayer = parent.Map[mapIndex].Layer;

                    if (parent.Handle.Layer != -1)
                    {
                        parentLayer += parent.Handle.Layer;
                    }

                    if (parent.TopLevelImage.LayerCount <= parentLayer)
                    {
                        Logger.PrintWarning(LogClass.Gpu, "Tried to copy out of bounds layer to parent");
                    }


                    // Calculate parent level
                    int parentLevel = parent.Map[mapIndex].Level + parent.Handle.Level;

                    if (parent.TopLevelImage.MaxMipmapLevel <= parentLevel)
                    {
                        Logger.PrintWarning(LogClass.Gpu, "Tried to copy out of bounds layer to parent");
                    }

                    // Attach parent texture
                    if (!isParentLayered)
                    {
                        GL.FramebufferTexture(FramebufferTarget.DrawFramebuffer, parentAttachment, parent.Handle.ParentHandle, parentLevel);
                    }
                    else
                    {
                        GL.FramebufferTextureLayer(FramebufferTarget.DrawFramebuffer, parentAttachment, parent.Handle.ParentHandle, parentLevel, parentLayer);
                    }

                    Logger.PrintInfo(LogClass.Gpu, $"Copying from {child.Handle.ParentHandle}:{child.Handle.Layer}:{child.Handle.Level} - {childLayer}:{childLevel} to " +
                                      $"{parent.Handle.ParentHandle}:{parent.Handle.Layer}:{parent.Handle.Level} - {parentLayer}:{parentLevel}");

                    // Do the copy
                    BlitFramebufferFilter filter = BlitFramebufferFilter.Nearest;

                    if (child.HasColor)
                    {
                        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

                        filter = BlitFramebufferFilter.Linear;
                    }

                    ClearBufferMask mask = OglRenderTarget.GetClearMask(child);

                    GL.BlitFramebuffer(0, 0, parent.Image.Width, parent.Image.Height,
                                       0, 0, child.Image.Width,  child.Image.Height,
                                       mask, filter);

                    parent.Map[mapIndex].Image = child;
                }
            }


            /*if (copySubTextures)
            {
                Copy(child,
                    parent,
                    layer,
                    parent.Map[mapIndex].Level,
                    0,
                    0,
                    parent.Image.Width,
                    parent.Image.Height,
                    0,
                    0,
                    parent.Image.Width,
                    parent.Image.Height);
            }*/

            // Calculate base layer just copied
            int layer = baseLayer;

            if (!isParentLayered)
            {
                layer = -1;
            }
            else if (parent.Handle.Layer != -1)
            {
                layer += parent.Handle.Layer;
            }

            int level = baseLevel + parent.Handle.Level;

            // Update child refs
            /*for (int i = 0; i < child.Map.Length; i++)
            {
                if (child.Map[i].Image == null)
                    continue;
                deadTextures.Add(child.Map[i].Image.Handle.Handle);
                child.Map[i].Image.Handle = new TextureHandle(parent.Handle.Handle, child.Map[i].Layer + layer, child.Map[i].Level);
            }*/

            (PixelInternalFormat internalFormat, PixelFormat format, PixelType pixelType)  = OglEnumConverter.GetImageFormat(parent.TopLevelImage.Format);
            int view = GL.GenTexture();
            //GL.BindTexture(TextureTarget.Texture2D, view);

            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, child.Image.Width, child.Image.Height, 0, format, pixelType, IntPtr.Zero);
            GL.TextureView(view, TextureTarget.Texture2D, parent.Handle.ParentHandle, internalFormat, level, 1, layer, 1);

            HashSet<int> deadTextures = child.SetHandle(new TextureHandle(parent.Handle.ParentHandle, view, layer, level), internalFormat).ToHashSet();

            Logger.PrintInfo(LogClass.Gpu, "Deleting:");
            foreach (var deadTexture in deadTextures)
            {
                Logger.PrintInfo(LogClass.Gpu, $" {deadTexture}");
            }
            Logger.PrintInfo(LogClass.Gpu, "");

            GL.DeleteTextures(deadTextures.Count, deadTextures.ToArray());

            //child.Handle = new TextureHandle(parent.Handle.Handle, layer, parent.Map[mapIndex].Level);
            child.Parent = parent;
        }

        private static bool IsAstc(GalImageFormat format)
        {
            format &= GalImageFormat.FormatMask;

            return format > GalImageFormat.Astc2DStart && format < GalImageFormat.Astc2DEnd;
        }

        public bool TryGetImage(TextureKey key, out GalImage image)
        {
            if (_textureCache.TryGetValue(key, out ImageHandler cachedImage))
            {
                image = cachedImage.Image;

                return true;
            }

            image = default(GalImage);

            return false;
        }

        public bool TryGetImageHandler(TextureKey key, out ImageHandler cachedImage)
        {
            if (_textureCache.TryGetValue(key, out cachedImage))
            {
                return true;
            }

            cachedImage = null;

            return false;
        }

        public void Bind(int index, ImageHandler imageHandler)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + index);

            TextureTarget target = ImageUtils.GetTextureTarget(imageHandler.Image.TextureTarget);

            GL.BindTexture(target, imageHandler.Handle.ParentHandle);

            int[] swizzleRgba = new int[]
            {
                (int)OglEnumConverter.GetTextureSwizzle(imageHandler.Image.XSource),
                (int)OglEnumConverter.GetTextureSwizzle(imageHandler.Image.YSource),
                (int)OglEnumConverter.GetTextureSwizzle(imageHandler.Image.ZSource),
                (int)OglEnumConverter.GetTextureSwizzle(imageHandler.Image.WSource)
            };

            GL.TexParameter(target, TextureParameterName.TextureSwizzleRgba, swizzleRgba);
        }

        public void SetSampler(GalImage image, GalTextureSampler sampler)
        {
            int wrapS = (int)OglEnumConverter.GetTextureWrapMode(sampler.AddressU);
            int wrapT = (int)OglEnumConverter.GetTextureWrapMode(sampler.AddressV);
            int wrapR = (int)OglEnumConverter.GetTextureWrapMode(sampler.AddressP);

            int minFilter = (int)OglEnumConverter.GetTextureMinFilter(sampler.MinFilter, sampler.MipFilter);
            int magFilter = (int)OglEnumConverter.GetTextureMagFilter(sampler.MagFilter);

            TextureTarget target = ImageUtils.GetTextureTarget(image.TextureTarget);

            GL.TexParameter(target, TextureParameterName.TextureWrapS, wrapS);
            GL.TexParameter(target, TextureParameterName.TextureWrapT, wrapT);
            GL.TexParameter(target, TextureParameterName.TextureWrapR, wrapR);

            GL.TexParameter(target, TextureParameterName.TextureMinFilter, minFilter);
            GL.TexParameter(target, TextureParameterName.TextureMagFilter, magFilter);

            GL.TexParameter(target, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(target, TextureParameterName.TextureMaxLevel, Math.Max(image.MaxMipmapLevel - 1, 0));
            /*GL.TexParameter(target, TextureParameterName.TextureMinLod, 0);
            GL.TexParameter(target, TextureParameterName.TextureMaxLod, image.MaxMipmapLevel);*/

            float[] color = new float[]
            {
                sampler.BorderColor.Red,
                sampler.BorderColor.Green,
                sampler.BorderColor.Blue,
                sampler.BorderColor.Alpha
            };

            GL.TexParameter(target, TextureParameterName.TextureBorderColor, color);

            if (sampler.DepthCompare)
            {
                GL.TexParameter(target, TextureParameterName.TextureCompareMode, (int)All.CompareRToTexture);
                GL.TexParameter(target, TextureParameterName.TextureCompareFunc, (int)OglEnumConverter.GetDepthCompareFunc(sampler.DepthCompareFunc));
            }
            else
            {
                GL.TexParameter(target, TextureParameterName.TextureCompareMode, (int)All.None);
                GL.TexParameter(target, TextureParameterName.TextureCompareFunc, (int)All.Never);
            }
        }
    }
}
