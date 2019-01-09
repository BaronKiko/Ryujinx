using ChocolArm64.Translation;

namespace ChocolArm64.Instructions
{
    delegate void InstEmitter(ILEmitterCtx context);

    delegate void InstEmitter86(ILEmitterCtx context);
}