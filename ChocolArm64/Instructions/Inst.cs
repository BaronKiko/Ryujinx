using System;
using ChocolArm64.Intructions86;

namespace ChocolArm64.Instructions
{
    struct Inst
    {
        public InstInterpreter Interpreter { get; private set; }
        public InstEmitter     Emitter     { get; private set; }
        public InstEmitter86   Emitter86   { get; private set; }
        public Type            Type        { get; private set; }

        public static Inst Undefined => new Inst(null, InstEmit.Und, InstEmit86.Und, null);

        public Inst(InstInterpreter interpreter, InstEmitter emitter, InstEmitter86 emitter86, Type type)
        {
            Interpreter = interpreter;
            Emitter     = emitter;
            Emitter86   = emitter86;
            Type        = type;
        }
    }
}