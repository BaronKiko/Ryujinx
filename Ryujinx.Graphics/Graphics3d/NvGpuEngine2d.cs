using Ryujinx.Graphics.Gal;
using Ryujinx.Graphics.Gal.OpenGL;
using Ryujinx.Graphics.Memory;
using Ryujinx.Graphics.Texture;
using Ryujinx.Profiler;

namespace Ryujinx.Graphics.Graphics3d
{
    class NvGpuEngine2d : INvGpuEngine
    {
        private enum CopyOperation
        {
            SrcCopyAnd,
            RopAnd,
            Blend,
            SrcCopy,
            Rop,
            SrcCopyPremult,
            BlendPremult
        }

        public int[] Registers { get; private set; }

        private NvGpu _gpu;

        public NvGpuEngine2d(NvGpu gpu)
        {
            _gpu = gpu;

            Registers = new int[0x238];
        }

        public void CallMethod(NvGpuVmm vmm, GpuMethodCall methCall)
        {
            WriteRegister(methCall);

            if ((NvGpuEngine2dReg)methCall.Method == NvGpuEngine2dReg.BlitSrcYInt)
            {
                TextureCopy(vmm);
            }
        }

        private void TextureCopy(NvGpuVmm vmm)
        {
            Profile.Begin(Profiles.GPU.Engine2d.TextureCopy);

            CopyOperation operation = (CopyOperation)ReadRegister(NvGpuEngine2dReg.CopyOperation);

            int  dstFormat = ReadRegister(NvGpuEngine2dReg.DstFormat);
            bool dstLinear = ReadRegister(NvGpuEngine2dReg.DstLinear) != 0;
            int  dstWidth  = ReadRegister(NvGpuEngine2dReg.DstWidth);
            int  dstHeight = ReadRegister(NvGpuEngine2dReg.DstHeight);
            int  dstDepth  = ReadRegister(NvGpuEngine2dReg.DstDepth);

            // No idea what this register is used for. It should be the total number of layers however that's useless to us as the texture cache will handle that for us. Same for srcLayer
            int dstLayer   = ReadRegister(NvGpuEngine2dReg.DstLayer);

            int  dstPitch  = ReadRegister(NvGpuEngine2dReg.DstPitch);
            int  dstBlkDim = ReadRegister(NvGpuEngine2dReg.DstBlockDimensions);

            int  srcFormat = ReadRegister(NvGpuEngine2dReg.SrcFormat);
            bool srcLinear = ReadRegister(NvGpuEngine2dReg.SrcLinear) != 0;
            int  srcWidth  = ReadRegister(NvGpuEngine2dReg.SrcWidth);
            int  srcHeight = ReadRegister(NvGpuEngine2dReg.SrcHeight);
            int  srcDepth  = ReadRegister(NvGpuEngine2dReg.SrcDepth);
            int  srcLayer  = ReadRegister(NvGpuEngine2dReg.SrcLayer);
            int  srcPitch  = ReadRegister(NvGpuEngine2dReg.SrcPitch);
            int  srcBlkDim = ReadRegister(NvGpuEngine2dReg.SrcBlockDimensions);

            int dstBlitX = ReadRegister(NvGpuEngine2dReg.BlitDstX);
            int dstBlitY = ReadRegister(NvGpuEngine2dReg.BlitDstY);
            int dstBlitW = ReadRegister(NvGpuEngine2dReg.BlitDstW);
            int dstBlitH = ReadRegister(NvGpuEngine2dReg.BlitDstH);

            long blitDuDx = ReadRegisterFixed1_31_32(NvGpuEngine2dReg.BlitDuDxFract);
            long blitDvDy = ReadRegisterFixed1_31_32(NvGpuEngine2dReg.BlitDvDyFract);

            long srcBlitX = ReadRegisterFixed1_31_32(NvGpuEngine2dReg.BlitSrcXFract);
            long srcBlitY = ReadRegisterFixed1_31_32(NvGpuEngine2dReg.BlitSrcYFract);

            GalImageFormat srcImgFormat = ImageUtils.ConvertSurface((GalSurfaceFormat)srcFormat);
            GalImageFormat dstImgFormat = ImageUtils.ConvertSurface((GalSurfaceFormat)dstFormat);

            GalMemoryLayout srcLayout = GetLayout(srcLinear);
            GalMemoryLayout dstLayout = GetLayout(dstLinear);

            int srcBlockHeight = 1 << ((srcBlkDim >> 4) & 0xf);
            int dstBlockHeight = 1 << ((dstBlkDim >> 4) & 0xf);

            long srcAddress = MakeInt64From2xInt32(NvGpuEngine2dReg.SrcAddress);
            long dstAddress = MakeInt64From2xInt32(NvGpuEngine2dReg.DstAddress);

            long srcKey = vmm.GetPhysicalAddress(srcAddress);
            long dstKey = vmm.GetPhysicalAddress(dstAddress);

            GalImage srcTexture = new GalImage(
                srcWidth,
                srcHeight,
                1, srcDepth, 1,
                srcBlockHeight, 1,
                srcLayout,
                srcImgFormat,
                GalTextureTarget.TwoD);

            GalImage dstTexture = new GalImage(
                dstWidth,
                dstHeight,
                1, dstDepth, 1,
                dstBlockHeight, 1,
                dstLayout,
                dstImgFormat,
                GalTextureTarget.TwoD);

            int srcSize = srcTexture.Size;
            int dstSize = dstTexture.Size;

            srcTexture.Pitch = srcPitch;
            dstTexture.Pitch = dstPitch;

            _gpu.ResourceManager.SendTexture(vmm, new TextureKey(srcKey, srcSize, GalTextureTarget.TwoD), srcTexture);
            _gpu.ResourceManager.SendTexture(vmm, new TextureKey(dstKey, dstSize, GalTextureTarget.TwoD), dstTexture);

            int srcBlitX1 = (int)(srcBlitX >> 32);
            int srcBlitY1 = (int)(srcBlitY >> 32);

            int srcBlitX2 = (int)(srcBlitX + dstBlitW * blitDuDx >> 32);
            int srcBlitY2 = (int)(srcBlitY + dstBlitH * blitDvDy >> 32);

            _gpu.Renderer.RenderTarget.Copy(
                srcTexture,
                dstTexture,
                srcKey,
                srcSize,
                dstKey,
                dstSize,
                srcBlitX1,
                srcBlitY1,
                srcBlitX2,
                srcBlitY2,
                dstBlitX,
                dstBlitY,
                dstBlitX + dstBlitW,
                dstBlitY + dstBlitH);

            //Do a guest side copy aswell. This is necessary when
            //the texture is modified by the guest, however it doesn't
            //work when resources that the gpu can write to are copied,
            //like framebuffers.

            // FIXME: SUPPORT MULTILAYER CORRECTLY HERE (this will cause weird stuffs on the first layer)
            ImageUtils.CopyTexture(
                vmm,
                srcTexture,
                dstTexture,
                srcAddress,
                dstAddress,
                srcBlitX1,
                srcBlitY1,
                dstBlitX,
                dstBlitY,
                dstBlitW,
                dstBlitH);

            vmm.IsRegionModified(dstKey, ImageUtils.GetSize(dstTexture), NvGpuBufferType.Texture);
        
            Profile.End(Profiles.GPU.Engine2d.TextureCopy);
        }

        private static GalMemoryLayout GetLayout(bool linear)
        {
            return linear
                ? GalMemoryLayout.Pitch
                : GalMemoryLayout.BlockLinear;
        }

        private long MakeInt64From2xInt32(NvGpuEngine2dReg reg)
        {
            return
                (long)Registers[(int)reg + 0] << 32 |
                (uint)Registers[(int)reg + 1];
        }

        private void WriteRegister(GpuMethodCall methCall)
        {
            Registers[methCall.Method] = methCall.Argument;
        }

        private long ReadRegisterFixed1_31_32(NvGpuEngine2dReg reg)
        {
            long low  = (uint)ReadRegister(reg + 0);
            long high = (uint)ReadRegister(reg + 1);

            return low | (high << 32);
        }

        private int ReadRegister(NvGpuEngine2dReg reg)
        {
            return Registers[(int)reg];
        }
    }
}
