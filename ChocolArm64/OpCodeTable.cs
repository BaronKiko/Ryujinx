using ChocolArm64.Decoders;
using ChocolArm64.Decoders32;
using ChocolArm64.Instructions;
using ChocolArm64.Instructions32;
using ChocolArm64.State;
using System;
using System.Collections.Generic;
using ChocolArm64.Intructions86;

namespace ChocolArm64
{
    static class OpCodeTable
    {
        static OpCodeTable()
        {
#region "OpCode Table (AArch32)"
            //Integer
            SetA32("<<<<1010xxxxxxxxxxxxxxxxxxxxxxxx", A32InstInterpret.B,      typeof(A32OpCodeBImmAl));
            SetA32("<<<<1011xxxxxxxxxxxxxxxxxxxxxxxx", A32InstInterpret.Bl,     typeof(A32OpCodeBImmAl));
            SetA32("1111101xxxxxxxxxxxxxxxxxxxxxxxxx", A32InstInterpret.Blx,    typeof(A32OpCodeBImmAl));
#endregion

#region "OpCode Table (AArch64)"
            //Integer
            SetA64("x0011010000xxxxx000000xxxxxxxxxx", InstEmit.Adc,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x0111010000xxxxx000000xxxxxxxxxx", InstEmit.Adcs,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x00100010xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Add,            InstEmit86.Add, typeof(OpCodeAluImm64));
            SetA64("00001011<<0xxxxx0xxxxxxxxxxxxxxx", InstEmit.Add,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10001011<<0xxxxxxxxxxxxxxxxxxxxx", InstEmit.Add,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x0001011001xxxxxxxx0xxxxxxxxxxxx", InstEmit.Add,            InstEmit86.Und, typeof(OpCodeAluRx64));
            SetA64("x0001011001xxxxxxxx100xxxxxxxxxx", InstEmit.Add,            InstEmit86.Und, typeof(OpCodeAluRx64));
            SetA64("x01100010xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Adds,           InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("00101011<<0xxxxx0xxxxxxxxxxxxxxx", InstEmit.Adds,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10101011<<0xxxxxxxxxxxxxxxxxxxxx", InstEmit.Adds,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x0101011001xxxxxxxx0xxxxxxxxxxxx", InstEmit.Adds,           InstEmit86.Und, typeof(OpCodeAluRx64));
            SetA64("x0101011001xxxxxxxx100xxxxxxxxxx", InstEmit.Adds,           InstEmit86.Und, typeof(OpCodeAluRx64));
            SetA64("0xx10000xxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Adr,            InstEmit86.Und, typeof(OpCodeAdr64));
            SetA64("1xx10000xxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Adrp,           InstEmit86.Und, typeof(OpCodeAdr64));
            SetA64("0001001000xxxxxxxxxxxxxxxxxxxxxx", InstEmit.And,            InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("100100100xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.And,            InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("00001010xx0xxxxx0xxxxxxxxxxxxxxx", InstEmit.And,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10001010xx0xxxxxxxxxxxxxxxxxxxxx", InstEmit.And,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("0111001000xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ands,           InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("111100100xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ands,           InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("01101010xx0xxxxx0xxxxxxxxxxxxxxx", InstEmit.Ands,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("11101010xx0xxxxxxxxxxxxxxxxxxxxx", InstEmit.Ands,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x0011010110xxxxx001010xxxxxxxxxx", InstEmit.Asrv,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("000101xxxxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.B,              InstEmit86.Und, typeof(OpCodeBImmAl64));
            SetA64("01010100xxxxxxxxxxxxxxxxxxx0xxxx", InstEmit.B_Cond,         InstEmit86.Und, typeof(OpCodeBImmCond64));
            SetA64("00110011000xxxxx0xxxxxxxxxxxxxxx", InstEmit.Bfm,            InstEmit86.Und, typeof(OpCodeBfm64));
            SetA64("1011001101xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Bfm,            InstEmit86.Und, typeof(OpCodeBfm64));
            SetA64("00001010xx1xxxxx0xxxxxxxxxxxxxxx", InstEmit.Bic,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10001010xx1xxxxxxxxxxxxxxxxxxxxx", InstEmit.Bic,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("01101010xx1xxxxx0xxxxxxxxxxxxxxx", InstEmit.Bics,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("11101010xx1xxxxxxxxxxxxxxxxxxxxx", InstEmit.Bics,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("100101xxxxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Bl,             InstEmit86.Und, typeof(OpCodeBImmAl64));
            SetA64("1101011000111111000000xxxxx00000", InstEmit.Blr,            InstEmit86.Und, typeof(OpCodeBReg64));
            SetA64("1101011000011111000000xxxxx00000", InstEmit.Br,             InstEmit86.Und, typeof(OpCodeBReg64));
            SetA64("11010100001xxxxxxxxxxxxxxxx00000", InstEmit.Brk,            InstEmit86.Und, typeof(OpCodeException64));
            SetA64("x0110101xxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Cbnz,           InstEmit86.Und, typeof(OpCodeBImmCmp64));
            SetA64("x0110100xxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Cbz,            InstEmit86.Und, typeof(OpCodeBImmCmp64));
            SetA64("x0111010010xxxxxxxxx10xxxxx0xxxx", InstEmit.Ccmn,           InstEmit86.Und, typeof(OpCodeCcmpImm64));
            SetA64("x0111010010xxxxxxxxx00xxxxx0xxxx", InstEmit.Ccmn,           InstEmit86.Und, typeof(OpCodeCcmpReg64));
            SetA64("x1111010010xxxxxxxxx10xxxxx0xxxx", InstEmit.Ccmp,           InstEmit86.Und, typeof(OpCodeCcmpImm64));
            SetA64("x1111010010xxxxxxxxx00xxxxx0xxxx", InstEmit.Ccmp,           InstEmit86.Und, typeof(OpCodeCcmpReg64));
            SetA64("11010101000000110011xxxx01011111", InstEmit.Clrex,          InstEmit86.Und, typeof(OpCodeSystem64));
            SetA64("x101101011000000000101xxxxxxxxxx", InstEmit.Cls,            InstEmit86.Und, typeof(OpCodeAlu64));
            SetA64("x101101011000000000100xxxxxxxxxx", InstEmit.Clz,            InstEmit86.Und, typeof(OpCodeAlu64));
            SetA64("00011010110xxxxx010000xxxxxxxxxx", InstEmit.Crc32b,         InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("00011010110xxxxx010001xxxxxxxxxx", InstEmit.Crc32h,         InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("00011010110xxxxx010010xxxxxxxxxx", InstEmit.Crc32w,         InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10011010110xxxxx010011xxxxxxxxxx", InstEmit.Crc32x,         InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("00011010110xxxxx010100xxxxxxxxxx", InstEmit.Crc32cb,        InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("00011010110xxxxx010101xxxxxxxxxx", InstEmit.Crc32ch,        InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("00011010110xxxxx010110xxxxxxxxxx", InstEmit.Crc32cw,        InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10011010110xxxxx010111xxxxxxxxxx", InstEmit.Crc32cx,        InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x0011010100xxxxxxxxx00xxxxxxxxxx", InstEmit.Csel,           InstEmit86.Und, typeof(OpCodeCsel64));
            SetA64("x0011010100xxxxxxxxx01xxxxxxxxxx", InstEmit.Csinc,          InstEmit86.Und, typeof(OpCodeCsel64));
            SetA64("x1011010100xxxxxxxxx00xxxxxxxxxx", InstEmit.Csinv,          InstEmit86.Und, typeof(OpCodeCsel64));
            SetA64("x1011010100xxxxxxxxx01xxxxxxxxxx", InstEmit.Csneg,          InstEmit86.Und, typeof(OpCodeCsel64));
            SetA64("11010101000000110011xxxx10111111", InstEmit.Dmb,            InstEmit86.Und, typeof(OpCodeSystem64));
            SetA64("11010101000000110011xxxx10011111", InstEmit.Dsb,            InstEmit86.Und, typeof(OpCodeSystem64));
            SetA64("01001010xx1xxxxx0xxxxxxxxxxxxxxx", InstEmit.Eon,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("11001010xx1xxxxxxxxxxxxxxxxxxxxx", InstEmit.Eon,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("0101001000xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Eor,            InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("110100100xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Eor,            InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("01001010xx0xxxxx0xxxxxxxxxxxxxxx", InstEmit.Eor,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("11001010xx0xxxxxxxxxxxxxxxxxxxxx", InstEmit.Eor,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("00010011100xxxxx0xxxxxxxxxxxxxxx", InstEmit.Extr,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10010011110xxxxxxxxxxxxxxxxxxxxx", InstEmit.Extr,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("11010101000000110010xxxxxxx11111", InstEmit.Hint,           InstEmit86.Und, typeof(OpCodeSystem64));
            SetA64("11010101000000110011xxxx11011111", InstEmit.Isb,            InstEmit86.Und, typeof(OpCodeSystem64));
            SetA64("xx001000110xxxxx1xxxxxxxxxxxxxxx", InstEmit.Ldar,           InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("1x001000011xxxxx1xxxxxxxxxxxxxxx", InstEmit.Ldaxp,          InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("xx001000010xxxxx1xxxxxxxxxxxxxxx", InstEmit.Ldaxr,          InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("<<10100xx1xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldp,            InstEmit86.Und, typeof(OpCodeMemPair64));
            SetA64("xx111000010xxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldr,            InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("xx11100101xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldr,            InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("xx111000011xxxxxxxxx10xxxxxxxxxx", InstEmit.Ldr,            InstEmit86.Und, typeof(OpCodeMemReg64));
            SetA64("xx011000xxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldr_Literal,    InstEmit86.Und, typeof(OpCodeMemLit64));
            SetA64("0x1110001x0xxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldrs,           InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("0x1110011xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldrs,           InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("10111000100xxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldrs,           InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("1011100110xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldrs,           InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("0x1110001x1xxxxxxxxx10xxxxxxxxxx", InstEmit.Ldrs,           InstEmit86.Und, typeof(OpCodeMemReg64));
            SetA64("10111000101xxxxxxxxx10xxxxxxxxxx", InstEmit.Ldrs,           InstEmit86.Und, typeof(OpCodeMemReg64));
            SetA64("xx001000010xxxxx0xxxxxxxxxxxxxxx", InstEmit.Ldxr,           InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("1x001000011xxxxx0xxxxxxxxxxxxxxx", InstEmit.Ldxp,           InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("x0011010110xxxxx001000xxxxxxxxxx", InstEmit.Lslv,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x0011010110xxxxx001001xxxxxxxxxx", InstEmit.Lsrv,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x0011011000xxxxx0xxxxxxxxxxxxxxx", InstEmit.Madd,           InstEmit86.Und, typeof(OpCodeMul64));
            SetA64("0111001010xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Movk,           InstEmit86.Und, typeof(OpCodeMov64));
            SetA64("111100101xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Movk,           InstEmit86.Und, typeof(OpCodeMov64));
            SetA64("0001001010xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Movn,           InstEmit86.Und, typeof(OpCodeMov64));
            SetA64("100100101xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Movn,           InstEmit86.Und, typeof(OpCodeMov64));
            SetA64("0101001010xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Movz,           InstEmit86.Und, typeof(OpCodeMov64));
            SetA64("110100101xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Movz,           InstEmit86.Und, typeof(OpCodeMov64));
            SetA64("110101010011xxxxxxxxxxxxxxxxxxxx", InstEmit.Mrs,            InstEmit86.Und, typeof(OpCodeSystem64));
            SetA64("110101010001xxxxxxxxxxxxxxxxxxxx", InstEmit.Msr,            InstEmit86.Und, typeof(OpCodeSystem64));
            SetA64("x0011011000xxxxx1xxxxxxxxxxxxxxx", InstEmit.Msub,           InstEmit86.Und, typeof(OpCodeMul64));
            SetA64("11010101000000110010000000011111", InstEmit.Nop,            InstEmit86.Und, typeof(OpCodeSystem64));
            SetA64("00101010xx1xxxxx0xxxxxxxxxxxxxxx", InstEmit.Orn,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10101010xx1xxxxxxxxxxxxxxxxxxxxx", InstEmit.Orn,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("0011001000xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Orr,            InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("101100100xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Orr,            InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("00101010xx0xxxxx0xxxxxxxxxxxxxxx", InstEmit.Orr,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10101010xx0xxxxxxxxxxxxxxxxxxxxx", InstEmit.Orr,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("1111100110xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Pfrm,           InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("11111000100xxxxxxxxx00xxxxxxxxxx", InstEmit.Pfrm,           InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("11011000xxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Pfrm,           InstEmit86.Und, typeof(OpCodeMemLit64));
            SetA64("x101101011000000000000xxxxxxxxxx", InstEmit.Rbit,           InstEmit86.Und, typeof(OpCodeAlu64));
            SetA64("1101011001011111000000xxxxx00000", InstEmit.Ret,            InstEmit86.Und, typeof(OpCodeBReg64));
            SetA64("x101101011000000000001xxxxxxxxxx", InstEmit.Rev16,          InstEmit86.Und, typeof(OpCodeAlu64));
            SetA64("x101101011000000000010xxxxxxxxxx", InstEmit.Rev32,          InstEmit86.Und, typeof(OpCodeAlu64));
            SetA64("1101101011000000000011xxxxxxxxxx", InstEmit.Rev64,          InstEmit86.Und, typeof(OpCodeAlu64));
            SetA64("x0011010110xxxxx001011xxxxxxxxxx", InstEmit.Rorv,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x1011010000xxxxx000000xxxxxxxxxx", InstEmit.Sbc,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x1111010000xxxxx000000xxxxxxxxxx", InstEmit.Sbcs,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("00010011000xxxxx0xxxxxxxxxxxxxxx", InstEmit.Sbfm,           InstEmit86.Und, typeof(OpCodeBfm64));
            SetA64("1001001101xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Sbfm,           InstEmit86.Und, typeof(OpCodeBfm64));
            SetA64("x0011010110xxxxx000011xxxxxxxxxx", InstEmit.Sdiv,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10011011001xxxxx0xxxxxxxxxxxxxxx", InstEmit.Smaddl,         InstEmit86.Und, typeof(OpCodeMul64));
            SetA64("10011011001xxxxx1xxxxxxxxxxxxxxx", InstEmit.Smsubl,         InstEmit86.Und, typeof(OpCodeMul64));
            SetA64("10011011010xxxxx0xxxxxxxxxxxxxxx", InstEmit.Smulh,          InstEmit86.Und, typeof(OpCodeMul64));
            SetA64("xx001000100xxxxx1xxxxxxxxxxxxxxx", InstEmit.Stlr,           InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("1x001000001xxxxx1xxxxxxxxxxxxxxx", InstEmit.Stlxp,          InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("xx001000000xxxxx1xxxxxxxxxxxxxxx", InstEmit.Stlxr,          InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("x010100xx0xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Stp,            InstEmit86.Und, typeof(OpCodeMemPair64));
            SetA64("xx111000000xxxxxxxxxxxxxxxxxxxxx", InstEmit.Str,            InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("xx11100100xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Str,            InstEmit86.Und, typeof(OpCodeMemImm64));
            SetA64("xx111000001xxxxxxxxx10xxxxxxxxxx", InstEmit.Str,            InstEmit86.Und, typeof(OpCodeMemReg64));
            SetA64("1x001000001xxxxx0xxxxxxxxxxxxxxx", InstEmit.Stxp,           InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("xx001000000xxxxx0xxxxxxxxxxxxxxx", InstEmit.Stxr,           InstEmit86.Und, typeof(OpCodeMemEx64));
            SetA64("x10100010xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Sub,            InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("01001011<<0xxxxx0xxxxxxxxxxxxxxx", InstEmit.Sub,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("11001011<<0xxxxxxxxxxxxxxxxxxxxx", InstEmit.Sub,            InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x1001011001xxxxxxxx0xxxxxxxxxxxx", InstEmit.Sub,            InstEmit86.Und, typeof(OpCodeAluRx64));
            SetA64("x1001011001xxxxxxxx100xxxxxxxxxx", InstEmit.Sub,            InstEmit86.Und, typeof(OpCodeAluRx64));
            SetA64("x11100010xxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Subs,           InstEmit86.Und, typeof(OpCodeAluImm64));
            SetA64("01101011<<0xxxxx0xxxxxxxxxxxxxxx", InstEmit.Subs,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("11101011<<0xxxxxxxxxxxxxxxxxxxxx", InstEmit.Subs,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("x1101011001xxxxxxxx0xxxxxxxxxxxx", InstEmit.Subs,           InstEmit86.Und, typeof(OpCodeAluRx64));
            SetA64("x1101011001xxxxxxxx100xxxxxxxxxx", InstEmit.Subs,           InstEmit86.Und, typeof(OpCodeAluRx64));
            SetA64("11010100000xxxxxxxxxxxxxxxx00001", InstEmit.Svc,            InstEmit86.Und, typeof(OpCodeException64));
            SetA64("1101010100001xxxxxxxxxxxxxxxxxxx", InstEmit.Sys,            InstEmit86.Und, typeof(OpCodeSystem64));
            SetA64("x0110111xxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Tbnz,           InstEmit86.Und, typeof(OpCodeBImmTest64));
            SetA64("x0110110xxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Tbz,            InstEmit86.Und, typeof(OpCodeBImmTest64));
            SetA64("01010011000xxxxx0xxxxxxxxxxxxxxx", InstEmit.Ubfm,           InstEmit86.Und, typeof(OpCodeBfm64));
            SetA64("1101001101xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ubfm,           InstEmit86.Und, typeof(OpCodeBfm64));
            SetA64("x0011010110xxxxx000010xxxxxxxxxx", InstEmit.Udiv,           InstEmit86.Und, typeof(OpCodeAluRs64));
            SetA64("10011011101xxxxx0xxxxxxxxxxxxxxx", InstEmit.Umaddl,         InstEmit86.Und, typeof(OpCodeMul64));
            SetA64("10011011101xxxxx1xxxxxxxxxxxxxxx", InstEmit.Umsubl,         InstEmit86.Und, typeof(OpCodeMul64));
            SetA64("10011011110xxxxx0xxxxxxxxxxxxxxx", InstEmit.Umulh,          InstEmit86.Und, typeof(OpCodeMul64));

            //Vector
            SetA64("0101111011100000101110xxxxxxxxxx", InstEmit.Abs_S,          InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>001110<<100000101110xxxxxxxxxx", InstEmit.Abs_V,          InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01011110111xxxxx100001xxxxxxxxxx", InstEmit.Add_S,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<1xxxxx100001xxxxxxxxxx", InstEmit.Add_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx010000xxxxxxxxxx", InstEmit.Addhn_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111011110001101110xxxxxxxxxx", InstEmit.Addp_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>001110<<1xxxxx101111xxxxxxxxxx", InstEmit.Addp_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000011100x110001101110xxxxxxxxxx", InstEmit.Addv_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01001110<<110001101110xxxxxxxxxx", InstEmit.Addv_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0100111000101000010110xxxxxxxxxx", InstEmit.Aesd_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0100111000101000010010xxxxxxxxxx", InstEmit.Aese_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0100111000101000011110xxxxxxxxxx", InstEmit.Aesimc_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0100111000101000011010xxxxxxxxxx", InstEmit.Aesmc_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x001110001xxxxx000111xxxxxxxxxx", InstEmit.And_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110011xxxxx000111xxxxxxxxxx", InstEmit.Bic_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x10111100000xxx<<x101xxxxxxxxxx", InstEmit.Bic_Vi,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("0x101110111xxxxx000111xxxxxxxxxx", InstEmit.Bif_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110101xxxxx000111xxxxxxxxxx", InstEmit.Bit_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110011xxxxx000111xxxxxxxxxx", InstEmit.Bsl_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<100000010010xxxxxxxxxx", InstEmit.Cls_V,          InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x101110<<100000010010xxxxxxxxxx", InstEmit.Clz_V,          InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01111110111xxxxx100011xxxxxxxxxx", InstEmit.Cmeq_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111011100000100110xxxxxxxxxx", InstEmit.Cmeq_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>101110<<1xxxxx100011xxxxxxxxxx", InstEmit.Cmeq_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<100000100110xxxxxxxxxx", InstEmit.Cmeq_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01011110111xxxxx001111xxxxxxxxxx", InstEmit.Cmge_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0111111011100000100010xxxxxxxxxx", InstEmit.Cmge_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>001110<<1xxxxx001111xxxxxxxxxx", InstEmit.Cmge_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>101110<<100000100010xxxxxxxxxx", InstEmit.Cmge_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01011110111xxxxx001101xxxxxxxxxx", InstEmit.Cmgt_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111011100000100010xxxxxxxxxx", InstEmit.Cmgt_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>001110<<1xxxxx001101xxxxxxxxxx", InstEmit.Cmgt_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<100000100010xxxxxxxxxx", InstEmit.Cmgt_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01111110111xxxxx001101xxxxxxxxxx", InstEmit.Cmhi_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>101110<<1xxxxx001101xxxxxxxxxx", InstEmit.Cmhi_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01111110111xxxxx001111xxxxxxxxxx", InstEmit.Cmhs_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>101110<<1xxxxx001111xxxxxxxxxx", InstEmit.Cmhs_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0111111011100000100110xxxxxxxxxx", InstEmit.Cmle_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>101110<<100000100110xxxxxxxxxx", InstEmit.Cmle_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0101111011100000101010xxxxxxxxxx", InstEmit.Cmlt_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>001110<<100000101010xxxxxxxxxx", InstEmit.Cmlt_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01011110111xxxxx100011xxxxxxxxxx", InstEmit.Cmtst_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<1xxxxx100011xxxxxxxxxx", InstEmit.Cmtst_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x00111000100000010110xxxxxxxxxx", InstEmit.Cnt_V,          InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>001110000x<>>>000011xxxxxxxxxx", InstEmit.Dup_Gp,         InstEmit86.Und, typeof(OpCodeSimdIns64));
            SetA64("01011110000xxxxx000001xxxxxxxxxx", InstEmit.Dup_S,          InstEmit86.Und, typeof(OpCodeSimdIns64));
            SetA64("0>001110000x<>>>000001xxxxxxxxxx", InstEmit.Dup_V,          InstEmit86.Und, typeof(OpCodeSimdIns64));
            SetA64("0x101110001xxxxx000111xxxxxxxxxx", InstEmit.Eor_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>101110000xxxxx0<xxx0xxxxxxxxxx", InstEmit.Ext_V,          InstEmit86.Und, typeof(OpCodeSimdExt64));
            SetA64("011111101x1xxxxx110101xxxxxxxxxx", InstEmit.Fabd_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>1011101<1xxxxx110101xxxxxxxxxx", InstEmit.Fabd_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x100000110000xxxxxxxxxx", InstEmit.Fabs_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011101<100000111110xxxxxxxxxx", InstEmit.Fabs_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x1xxxxx001010xxxxxxxxxx", InstEmit.Fadd_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011100<1xxxxx110101xxxxxxxxxx", InstEmit.Fadd_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("011111100x110000110110xxxxxxxxxx", InstEmit.Faddp_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011100<1xxxxx110101xxxxxxxxxx", InstEmit.Faddp_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x1xxxxxxxxx01xxxxx0xxxx", InstEmit.Fccmp_S,        InstEmit86.Und, typeof(OpCodeSimdFcond64));
            SetA64("000111100x1xxxxxxxxx01xxxxx1xxxx", InstEmit.Fccmpe_S,       InstEmit86.Und, typeof(OpCodeSimdFcond64));
            SetA64("010111100x1xxxxx111001xxxxxxxxxx", InstEmit.Fcmeq_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("010111101x100000110110xxxxxxxxxx", InstEmit.Fcmeq_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011100<1xxxxx111001xxxxxxxxxx", InstEmit.Fcmeq_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011101<100000110110xxxxxxxxxx", InstEmit.Fcmeq_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("011111100x1xxxxx111001xxxxxxxxxx", InstEmit.Fcmge_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("011111101x100000110010xxxxxxxxxx", InstEmit.Fcmge_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011100<1xxxxx111001xxxxxxxxxx", InstEmit.Fcmge_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>1011101<100000110010xxxxxxxxxx", InstEmit.Fcmge_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("011111101x1xxxxx111001xxxxxxxxxx", InstEmit.Fcmgt_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("010111101x100000110010xxxxxxxxxx", InstEmit.Fcmgt_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011101<1xxxxx111001xxxxxxxxxx", InstEmit.Fcmgt_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011101<100000110010xxxxxxxxxx", InstEmit.Fcmgt_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("011111101x100000110110xxxxxxxxxx", InstEmit.Fcmle_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011101<100000110110xxxxxxxxxx", InstEmit.Fcmle_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("010111101x100000111010xxxxxxxxxx", InstEmit.Fcmlt_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011101<100000111010xxxxxxxxxx", InstEmit.Fcmlt_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x1xxxxx001000xxxxx0x000", InstEmit.Fcmp_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x1xxxxx001000xxxxx1x000", InstEmit.Fcmpe_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x1xxxxxxxxx11xxxxxxxxxx", InstEmit.Fcsel_S,        InstEmit86.Und, typeof(OpCodeSimdFcond64));
            SetA64("000111100x10001xx10000xxxxxxxxxx", InstEmit.Fcvt_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("x00111100x100100000000xxxxxxxxxx", InstEmit.Fcvtas_Gp,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("x00111100x100101000000xxxxxxxxxx", InstEmit.Fcvtau_Gp,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("0x0011100x100001011110xxxxxxxxxx", InstEmit.Fcvtl_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("x00111100x110000000000xxxxxxxxxx", InstEmit.Fcvtms_Gp,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("x00111100x110001000000xxxxxxxxxx", InstEmit.Fcvtmu_Gp,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("0x0011100x100001011010xxxxxxxxxx", InstEmit.Fcvtn_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("010111100x100001101010xxxxxxxxxx", InstEmit.Fcvtns_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011100<100001101010xxxxxxxxxx", InstEmit.Fcvtns_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("011111100x100001101010xxxxxxxxxx", InstEmit.Fcvtnu_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011100<100001101010xxxxxxxxxx", InstEmit.Fcvtnu_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("x00111100x101000000000xxxxxxxxxx", InstEmit.Fcvtps_Gp,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("x00111100x101001000000xxxxxxxxxx", InstEmit.Fcvtpu_Gp,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("x00111100x111000000000xxxxxxxxxx", InstEmit.Fcvtzs_Gp,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("x00111100x011000xxxxxxxxxxxxxxxx", InstEmit.Fcvtzs_Gp_Fixed,InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("010111101x100001101110xxxxxxxxxx", InstEmit.Fcvtzs_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011101<100001101110xxxxxxxxxx", InstEmit.Fcvtzs_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x0011110>>xxxxx111111xxxxxxxxxx", InstEmit.Fcvtzs_V,       InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("x00111100x111001000000xxxxxxxxxx", InstEmit.Fcvtzu_Gp,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("x00111100x011001xxxxxxxxxxxxxxxx", InstEmit.Fcvtzu_Gp_Fixed,InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("011111101x100001101110xxxxxxxxxx", InstEmit.Fcvtzu_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011101<100001101110xxxxxxxxxx", InstEmit.Fcvtzu_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x1011110>>xxxxx111111xxxxxxxxxx", InstEmit.Fcvtzu_V,       InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("000111100x1xxxxx000110xxxxxxxxxx", InstEmit.Fdiv_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>1011100<1xxxxx111111xxxxxxxxxx", InstEmit.Fdiv_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111110x0xxxxx0xxxxxxxxxxxxxxx", InstEmit.Fmadd_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x1xxxxx010010xxxxxxxxxx", InstEmit.Fmax_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011100<1xxxxx111101xxxxxxxxxx", InstEmit.Fmax_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x1xxxxx011010xxxxxxxxxx", InstEmit.Fmaxnm_S,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011100<1xxxxx110001xxxxxxxxxx", InstEmit.Fmaxnm_V,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>1011100<1xxxxx111101xxxxxxxxxx", InstEmit.Fmaxp_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x1xxxxx010110xxxxxxxxxx", InstEmit.Fmin_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011101<1xxxxx111101xxxxxxxxxx", InstEmit.Fmin_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x1xxxxx011110xxxxxxxxxx", InstEmit.Fminnm_S,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011101<1xxxxx110001xxxxxxxxxx", InstEmit.Fminnm_V,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>1011101<1xxxxx111101xxxxxxxxxx", InstEmit.Fminp_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("010111111xxxxxxx0001x0xxxxxxxxxx", InstEmit.Fmla_Se,        InstEmit86.Und, typeof(OpCodeSimdRegElemF64));
            SetA64("0>0011100<1xxxxx110011xxxxxxxxxx", InstEmit.Fmla_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011111<xxxxxx0001x0xxxxxxxxxx", InstEmit.Fmla_Ve,        InstEmit86.Und, typeof(OpCodeSimdRegElemF64));
            SetA64("010111111xxxxxxx0101x0xxxxxxxxxx", InstEmit.Fmls_Se,        InstEmit86.Und, typeof(OpCodeSimdRegElemF64));
            SetA64("0>0011101<1xxxxx110011xxxxxxxxxx", InstEmit.Fmls_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011111<xxxxxx0101x0xxxxxxxxxx", InstEmit.Fmls_Ve,        InstEmit86.Und, typeof(OpCodeSimdRegElemF64));
            SetA64("000111100x100000010000xxxxxxxxxx", InstEmit.Fmov_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("00011110xx1xxxxxxxx100xxxxxxxxxx", InstEmit.Fmov_Si,        InstEmit86.Und, typeof(OpCodeSimdFmov64));
            SetA64("0xx0111100000xxx111101xxxxxxxxxx", InstEmit.Fmov_V,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("x00111100x100110000000xxxxxxxxxx", InstEmit.Fmov_Ftoi,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("x00111100x100111000000xxxxxxxxxx", InstEmit.Fmov_Itof,      InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("1001111010101110000000xxxxxxxxxx", InstEmit.Fmov_Ftoi1,     InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("1001111010101111000000xxxxxxxxxx", InstEmit.Fmov_Itof1,     InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("000111110x0xxxxx1xxxxxxxxxxxxxxx", InstEmit.Fmsub_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x1xxxxx000010xxxxxxxxxx", InstEmit.Fmul_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("010111111xxxxxxx1001x0xxxxxxxxxx", InstEmit.Fmul_Se,        InstEmit86.Und, typeof(OpCodeSimdRegElemF64));
            SetA64("0>1011100<1xxxxx110111xxxxxxxxxx", InstEmit.Fmul_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011111<xxxxxx1001x0xxxxxxxxxx", InstEmit.Fmul_Ve,        InstEmit86.Und, typeof(OpCodeSimdRegElemF64));
            SetA64("010111100x1xxxxx110111xxxxxxxxxx", InstEmit.Fmulx_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("011111111xxxxxxx1001x0xxxxxxxxxx", InstEmit.Fmulx_Se,       InstEmit86.Und, typeof(OpCodeSimdRegElemF64));
            SetA64("0>0011100<1xxxxx110111xxxxxxxxxx", InstEmit.Fmulx_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>1011111<xxxxxx1001x0xxxxxxxxxx", InstEmit.Fmulx_Ve,       InstEmit86.Und, typeof(OpCodeSimdRegElemF64));
            SetA64("000111100x100001010000xxxxxxxxxx", InstEmit.Fneg_S,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011101<100000111110xxxxxxxxxx", InstEmit.Fneg_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111110x1xxxxx0xxxxxxxxxxxxxxx", InstEmit.Fnmadd_S,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111110x1xxxxx1xxxxxxxxxxxxxxx", InstEmit.Fnmsub_S,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x1xxxxx100010xxxxxxxxxx", InstEmit.Fnmul_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("010111101x100001110110xxxxxxxxxx", InstEmit.Frecpe_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011101<100001110110xxxxxxxxxx", InstEmit.Frecpe_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("010111100x1xxxxx111111xxxxxxxxxx", InstEmit.Frecps_S,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011100<1xxxxx111111xxxxxxxxxx", InstEmit.Frecps_V,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("010111101x100001111110xxxxxxxxxx", InstEmit.Frecpx_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x100110010000xxxxxxxxxx", InstEmit.Frinta_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011100<100001100010xxxxxxxxxx", InstEmit.Frinta_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x100111110000xxxxxxxxxx", InstEmit.Frinti_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011101<100001100110xxxxxxxxxx", InstEmit.Frinti_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x100101010000xxxxxxxxxx", InstEmit.Frintm_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011100<100001100110xxxxxxxxxx", InstEmit.Frintm_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x100100010000xxxxxxxxxx", InstEmit.Frintn_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011100<100001100010xxxxxxxxxx", InstEmit.Frintn_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x100100110000xxxxxxxxxx", InstEmit.Frintp_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011101<100001100010xxxxxxxxxx", InstEmit.Frintp_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x100111010000xxxxxxxxxx", InstEmit.Frintx_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011100<100001100110xxxxxxxxxx", InstEmit.Frintx_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x100101110000xxxxxxxxxx", InstEmit.Frintz_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011101<100001100110xxxxxxxxxx", InstEmit.Frintz_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("011111101x100001110110xxxxxxxxxx", InstEmit.Frsqrte_S,      InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011101<100001110110xxxxxxxxxx", InstEmit.Frsqrte_V,      InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("010111101x1xxxxx111111xxxxxxxxxx", InstEmit.Frsqrts_S,      InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011101<1xxxxx111111xxxxxxxxxx", InstEmit.Frsqrts_V,      InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("000111100x100001110000xxxxxxxxxx", InstEmit.Fsqrt_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011101<100001111110xxxxxxxxxx", InstEmit.Fsqrt_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("000111100x1xxxxx001110xxxxxxxxxx", InstEmit.Fsub_S,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>0011101<1xxxxx110101xxxxxxxxxx", InstEmit.Fsub_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01001110000xxxxx000111xxxxxxxxxx", InstEmit.Ins_Gp,         InstEmit86.Und, typeof(OpCodeSimdIns64));
            SetA64("01101110000xxxxx0xxxx1xxxxxxxxxx", InstEmit.Ins_V,          InstEmit86.Und, typeof(OpCodeSimdIns64));
            SetA64("0x00110001000000xxxxxxxxxxxxxxxx", InstEmit.Ld__Vms,        InstEmit86.Und, typeof(OpCodeSimdMemMs64));
            SetA64("0x001100110xxxxxxxxxxxxxxxxxxxxx", InstEmit.Ld__Vms,        InstEmit86.Und, typeof(OpCodeSimdMemMs64));
            SetA64("0x00110101x00000xxxxxxxxxxxxxxxx", InstEmit.Ld__Vss,        InstEmit86.Und, typeof(OpCodeSimdMemSs64));
            SetA64("0x00110111xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ld__Vss,        InstEmit86.Und, typeof(OpCodeSimdMemSs64));
            SetA64("xx10110xx1xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldp,            InstEmit86.Und, typeof(OpCodeSimdMemPair64));
            SetA64("xx111100x10xxxxxxxxx00xxxxxxxxxx", InstEmit.Ldr,            InstEmit86.Und, typeof(OpCodeSimdMemImm64));
            SetA64("xx111100x10xxxxxxxxx01xxxxxxxxxx", InstEmit.Ldr,            InstEmit86.Und, typeof(OpCodeSimdMemImm64));
            SetA64("xx111100x10xxxxxxxxx11xxxxxxxxxx", InstEmit.Ldr,            InstEmit86.Und, typeof(OpCodeSimdMemImm64));
            SetA64("xx111101x1xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldr,            InstEmit86.Und, typeof(OpCodeSimdMemImm64));
            SetA64("xx111100x11xxxxxxxxx10xxxxxxxxxx", InstEmit.Ldr,            InstEmit86.Und, typeof(OpCodeSimdMemReg64));
            SetA64("xx011100xxxxxxxxxxxxxxxxxxxxxxxx", InstEmit.Ldr_Literal,    InstEmit86.Und, typeof(OpCodeSimdMemLit64));
            SetA64("0x001110<<1xxxxx100101xxxxxxxxxx", InstEmit.Mla_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101111xxxxxxxx0000x0xxxxxxxxxx", InstEmit.Mla_Ve,         InstEmit86.Und, typeof(OpCodeSimdRegElem64));
            SetA64("0x101110<<1xxxxx100101xxxxxxxxxx", InstEmit.Mls_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101111xxxxxxxx0100x0xxxxxxxxxx", InstEmit.Mls_Ve,         InstEmit86.Und, typeof(OpCodeSimdRegElem64));
            SetA64("0x00111100000xxx0xx001xxxxxxxxxx", InstEmit.Movi_V,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("0x00111100000xxx10x001xxxxxxxxxx", InstEmit.Movi_V,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("0x00111100000xxx110x01xxxxxxxxxx", InstEmit.Movi_V,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("0xx0111100000xxx111001xxxxxxxxxx", InstEmit.Movi_V,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("0x001110<<1xxxxx100111xxxxxxxxxx", InstEmit.Mul_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001111xxxxxxxx1000x0xxxxxxxxxx", InstEmit.Mul_Ve,         InstEmit86.Und, typeof(OpCodeSimdRegElem64));
            SetA64("0x10111100000xxx0xx001xxxxxxxxxx", InstEmit.Mvni_V,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("0x10111100000xxx10x001xxxxxxxxxx", InstEmit.Mvni_V,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("0x10111100000xxx110x01xxxxxxxxxx", InstEmit.Mvni_V,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("0111111011100000101110xxxxxxxxxx", InstEmit.Neg_S,          InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>101110<<100000101110xxxxxxxxxx", InstEmit.Neg_V,          InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x10111000100000010110xxxxxxxxxx", InstEmit.Not_V,          InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x001110111xxxxx000111xxxxxxxxxx", InstEmit.Orn_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110101xxxxx000111xxxxxxxxxx", InstEmit.Orr_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x00111100000xxx<<x101xxxxxxxxxx", InstEmit.Orr_Vi,         InstEmit86.Und, typeof(OpCodeSimdImm64));
            SetA64("0x101110<<1xxxxx010000xxxxxxxxxx", InstEmit.Raddhn_V,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x10111001100000010110xxxxxxxxxx", InstEmit.Rbit_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x00111000100000000110xxxxxxxxxx", InstEmit.Rev16_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x1011100x100000000010xxxxxxxxxx", InstEmit.Rev32_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x001110<<100000000010xxxxxxxxxx", InstEmit.Rev64_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x00111100>>>xxx100011xxxxxxxxxx", InstEmit.Rshrn_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x101110<<1xxxxx011000xxxxxxxxxx", InstEmit.Rsubhn_V,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx011111xxxxxxxxxx", InstEmit.Saba_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx010100xxxxxxxxxx", InstEmit.Sabal_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx011101xxxxxxxxxx", InstEmit.Sabd_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx011100xxxxxxxxxx", InstEmit.Sabdl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<100000011010xxxxxxxxxx", InstEmit.Sadalp_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x001110<<1xxxxx000000xxxxxxxxxx", InstEmit.Saddl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<100000001010xxxxxxxxxx", InstEmit.Saddlp_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x001110<<1xxxxx000100xxxxxxxxxx", InstEmit.Saddw_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("x00111100x100010000000xxxxxxxxxx", InstEmit.Scvtf_Gp,       InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("010111100x100001110110xxxxxxxxxx", InstEmit.Scvtf_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>0011100<100001110110xxxxxxxxxx", InstEmit.Scvtf_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01011110000xxxxx000000xxxxxxxxxx", InstEmit.Sha1c_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111000101000000010xxxxxxxxxx", InstEmit.Sha1h_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01011110000xxxxx001000xxxxxxxxxx", InstEmit.Sha1m_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01011110000xxxxx000100xxxxxxxxxx", InstEmit.Sha1p_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01011110000xxxxx001100xxxxxxxxxx", InstEmit.Sha1su0_V,      InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111000101000000110xxxxxxxxxx", InstEmit.Sha1su1_V,      InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01011110000xxxxx010000xxxxxxxxxx", InstEmit.Sha256h_V,      InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01011110000xxxxx010100xxxxxxxxxx", InstEmit.Sha256h2_V,     InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111000101000001010xxxxxxxxxx", InstEmit.Sha256su0_V,    InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01011110000xxxxx011000xxxxxxxxxx", InstEmit.Sha256su1_V,    InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx000001xxxxxxxxxx", InstEmit.Shadd_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111101xxxxxx010101xxxxxxxxxx", InstEmit.Shl_S,          InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x00111100>>>xxx010101xxxxxxxxxx", InstEmit.Shl_V,          InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0100111101xxxxxx010101xxxxxxxxxx", InstEmit.Shl_V,          InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x101110<<100001001110xxxxxxxxxx", InstEmit.Shll_V,         InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x00111100>>>xxx100001xxxxxxxxxx", InstEmit.Shrn_V,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x001110<<1xxxxx001001xxxxxxxxxx", InstEmit.Shsub_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x1011110>>>>xxx010101xxxxxxxxxx", InstEmit.Sli_V,          InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x001110<<1xxxxx011001xxxxxxxxxx", InstEmit.Smax_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx101001xxxxxxxxxx", InstEmit.Smaxp_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx011011xxxxxxxxxx", InstEmit.Smin_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx101011xxxxxxxxxx", InstEmit.Sminp_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx100000xxxxxxxxxx", InstEmit.Smlal_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx101000xxxxxxxxxx", InstEmit.Smlsl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110000xxxxx001011xxxxxxxxxx", InstEmit.Smov_S,         InstEmit86.Und, typeof(OpCodeSimdIns64));
            SetA64("0x001110<<1xxxxx110000xxxxxxxxxx", InstEmit.Smull_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01011110xx100000011110xxxxxxxxxx", InstEmit.Sqabs_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>001110<<100000011110xxxxxxxxxx", InstEmit.Sqabs_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01011110xx1xxxxx000011xxxxxxxxxx", InstEmit.Sqadd_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<1xxxxx000011xxxxxxxxxx", InstEmit.Sqadd_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01011110011xxxxx101101xxxxxxxxxx", InstEmit.Sqdmulh_S,      InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01011110101xxxxx101101xxxxxxxxxx", InstEmit.Sqdmulh_S,      InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110011xxxxx101101xxxxxxxxxx", InstEmit.Sqdmulh_V,      InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110101xxxxx101101xxxxxxxxxx", InstEmit.Sqdmulh_V,      InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01111110xx100000011110xxxxxxxxxx", InstEmit.Sqneg_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>101110<<100000011110xxxxxxxxxx", InstEmit.Sqneg_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01111110011xxxxx101101xxxxxxxxxx", InstEmit.Sqrdmulh_S,     InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01111110101xxxxx101101xxxxxxxxxx", InstEmit.Sqrdmulh_S,     InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110011xxxxx101101xxxxxxxxxx", InstEmit.Sqrdmulh_V,     InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110101xxxxx101101xxxxxxxxxx", InstEmit.Sqrdmulh_V,     InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<1xxxxx010111xxxxxxxxxx", InstEmit.Sqrshl_V,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111100>>>xxx100111xxxxxxxxxx", InstEmit.Sqrshrn_S,      InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x00111100>>>xxx100111xxxxxxxxxx", InstEmit.Sqrshrn_V,      InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0111111100>>>xxx100011xxxxxxxxxx", InstEmit.Sqrshrun_S,     InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x10111100>>>xxx100011xxxxxxxxxx", InstEmit.Sqrshrun_V,     InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0>001110<<1xxxxx010011xxxxxxxxxx", InstEmit.Sqshl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111100>>>xxx100101xxxxxxxxxx", InstEmit.Sqshrn_S,       InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x00111100>>>xxx100101xxxxxxxxxx", InstEmit.Sqshrn_V,       InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0111111100>>>xxx100001xxxxxxxxxx", InstEmit.Sqshrun_S,      InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x10111100>>>xxx100001xxxxxxxxxx", InstEmit.Sqshrun_V,      InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("01011110xx1xxxxx001011xxxxxxxxxx", InstEmit.Sqsub_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<1xxxxx001011xxxxxxxxxx", InstEmit.Sqsub_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01011110<<100001010010xxxxxxxxxx", InstEmit.Sqxtn_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x001110<<100001010010xxxxxxxxxx", InstEmit.Sqxtn_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01111110<<100001001010xxxxxxxxxx", InstEmit.Sqxtun_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x101110<<100001001010xxxxxxxxxx", InstEmit.Sqxtun_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x001110<<1xxxxx000101xxxxxxxxxx", InstEmit.Srhadd_V,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<1xxxxx010101xxxxxxxxxx", InstEmit.Srshl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0101111101xxxxxx001001xxxxxxxxxx", InstEmit.Srshr_S,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x00111100>>>xxx001001xxxxxxxxxx", InstEmit.Srshr_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0100111101xxxxxx001001xxxxxxxxxx", InstEmit.Srshr_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0101111101xxxxxx001101xxxxxxxxxx", InstEmit.Srsra_S,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x00111100>>>xxx001101xxxxxxxxxx", InstEmit.Srsra_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0100111101xxxxxx001101xxxxxxxxxx", InstEmit.Srsra_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0>001110<<1xxxxx010001xxxxxxxxxx", InstEmit.Sshl_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x00111100>>>xxx101001xxxxxxxxxx", InstEmit.Sshll_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0101111101xxxxxx000001xxxxxxxxxx", InstEmit.Sshr_S,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x00111100>>>xxx000001xxxxxxxxxx", InstEmit.Sshr_V,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0100111101xxxxxx000001xxxxxxxxxx", InstEmit.Sshr_V,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0101111101xxxxxx000101xxxxxxxxxx", InstEmit.Ssra_S,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x00111100>>>xxx000101xxxxxxxxxx", InstEmit.Ssra_V,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0100111101xxxxxx000101xxxxxxxxxx", InstEmit.Ssra_V,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x001110<<1xxxxx001000xxxxxxxxxx", InstEmit.Ssubl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx001100xxxxxxxxxx", InstEmit.Ssubw_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x00110000000000xxxxxxxxxxxxxxxx", InstEmit.St__Vms,        InstEmit86.Und, typeof(OpCodeSimdMemMs64));
            SetA64("0x001100100xxxxxxxxxxxxxxxxxxxxx", InstEmit.St__Vms,        InstEmit86.Und, typeof(OpCodeSimdMemMs64));
            SetA64("0x00110100x00000xxxxxxxxxxxxxxxx", InstEmit.St__Vss,        InstEmit86.Und, typeof(OpCodeSimdMemSs64));
            SetA64("0x00110110xxxxxxxxxxxxxxxxxxxxxx", InstEmit.St__Vss,        InstEmit86.Und, typeof(OpCodeSimdMemSs64));
            SetA64("xx10110xx0xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Stp,            InstEmit86.Und, typeof(OpCodeSimdMemPair64));
            SetA64("xx111100x00xxxxxxxxx00xxxxxxxxxx", InstEmit.Str,            InstEmit86.Und, typeof(OpCodeSimdMemImm64));
            SetA64("xx111100x00xxxxxxxxx01xxxxxxxxxx", InstEmit.Str,            InstEmit86.Und, typeof(OpCodeSimdMemImm64));
            SetA64("xx111100x00xxxxxxxxx11xxxxxxxxxx", InstEmit.Str,            InstEmit86.Und, typeof(OpCodeSimdMemImm64));
            SetA64("xx111101x0xxxxxxxxxxxxxxxxxxxxxx", InstEmit.Str,            InstEmit86.Und, typeof(OpCodeSimdMemImm64));
            SetA64("xx111100x01xxxxxxxxx10xxxxxxxxxx", InstEmit.Str,            InstEmit86.Und, typeof(OpCodeSimdMemReg64));
            SetA64("01111110111xxxxx100001xxxxxxxxxx", InstEmit.Sub_S,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>101110<<1xxxxx100001xxxxxxxxxx", InstEmit.Sub_V,          InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<1xxxxx011000xxxxxxxxxx", InstEmit.Subhn_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01011110xx100000001110xxxxxxxxxx", InstEmit.Suqadd_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>001110<<100000001110xxxxxxxxxx", InstEmit.Suqadd_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x001110000xxxxx0xx000xxxxxxxxxx", InstEmit.Tbl_V,          InstEmit86.Und, typeof(OpCodeSimdTbl64));
            SetA64("0>001110<<0xxxxx001010xxxxxxxxxx", InstEmit.Trn1_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<0xxxxx011010xxxxxxxxxx", InstEmit.Trn2_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx011111xxxxxxxxxx", InstEmit.Uaba_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx010100xxxxxxxxxx", InstEmit.Uabal_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx011101xxxxxxxxxx", InstEmit.Uabd_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx011100xxxxxxxxxx", InstEmit.Uabdl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<100000011010xxxxxxxxxx", InstEmit.Uadalp_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x101110<<1xxxxx000000xxxxxxxxxx", InstEmit.Uaddl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<100000001010xxxxxxxxxx", InstEmit.Uaddlp_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("001011100x110000001110xxxxxxxxxx", InstEmit.Uaddlv_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("01101110<<110000001110xxxxxxxxxx", InstEmit.Uaddlv_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x101110<<1xxxxx000100xxxxxxxxxx", InstEmit.Uaddw_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("x00111100x100011000000xxxxxxxxxx", InstEmit.Ucvtf_Gp,       InstEmit86.Und, typeof(OpCodeSimdCvt64));
            SetA64("011111100x100001110110xxxxxxxxxx", InstEmit.Ucvtf_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>1011100<100001110110xxxxxxxxxx", InstEmit.Ucvtf_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x101110<<1xxxxx000001xxxxxxxxxx", InstEmit.Uhadd_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx001001xxxxxxxxxx", InstEmit.Uhsub_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx011001xxxxxxxxxx", InstEmit.Umax_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx101001xxxxxxxxxx", InstEmit.Umaxp_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx011011xxxxxxxxxx", InstEmit.Umin_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx101011xxxxxxxxxx", InstEmit.Uminp_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx100000xxxxxxxxxx", InstEmit.Umlal_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx101000xxxxxxxxxx", InstEmit.Umlsl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110000xxxxx001111xxxxxxxxxx", InstEmit.Umov_S,         InstEmit86.Und, typeof(OpCodeSimdIns64));
            SetA64("0x101110<<1xxxxx110000xxxxxxxxxx", InstEmit.Umull_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01111110xx1xxxxx000011xxxxxxxxxx", InstEmit.Uqadd_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>101110<<1xxxxx000011xxxxxxxxxx", InstEmit.Uqadd_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>101110<<1xxxxx010111xxxxxxxxxx", InstEmit.Uqrshl_V,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0111111100>>>xxx100111xxxxxxxxxx", InstEmit.Uqrshrn_S,      InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x10111100>>>xxx100111xxxxxxxxxx", InstEmit.Uqrshrn_V,      InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0>101110<<1xxxxx010011xxxxxxxxxx", InstEmit.Uqshl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0111111100>>>xxx100101xxxxxxxxxx", InstEmit.Uqshrn_S,       InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x10111100>>>xxx100101xxxxxxxxxx", InstEmit.Uqshrn_V,       InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("01111110xx1xxxxx001011xxxxxxxxxx", InstEmit.Uqsub_S,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>101110<<1xxxxx001011xxxxxxxxxx", InstEmit.Uqsub_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("01111110<<100001010010xxxxxxxxxx", InstEmit.Uqxtn_S,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x101110<<100001010010xxxxxxxxxx", InstEmit.Uqxtn_V,        InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0x101110<<1xxxxx000101xxxxxxxxxx", InstEmit.Urhadd_V,       InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>101110<<1xxxxx010101xxxxxxxxxx", InstEmit.Urshl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0111111101xxxxxx001001xxxxxxxxxx", InstEmit.Urshr_S,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x10111100>>>xxx001001xxxxxxxxxx", InstEmit.Urshr_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0110111101xxxxxx001001xxxxxxxxxx", InstEmit.Urshr_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0111111101xxxxxx001101xxxxxxxxxx", InstEmit.Ursra_S,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x10111100>>>xxx001101xxxxxxxxxx", InstEmit.Ursra_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0110111101xxxxxx001101xxxxxxxxxx", InstEmit.Ursra_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0>101110<<1xxxxx010001xxxxxxxxxx", InstEmit.Ushl_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x10111100>>>xxx101001xxxxxxxxxx", InstEmit.Ushll_V,        InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0111111101xxxxxx000001xxxxxxxxxx", InstEmit.Ushr_S,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x10111100>>>xxx000001xxxxxxxxxx", InstEmit.Ushr_V,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0110111101xxxxxx000001xxxxxxxxxx", InstEmit.Ushr_V,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("01111110xx100000001110xxxxxxxxxx", InstEmit.Usqadd_S,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>101110<<100000001110xxxxxxxxxx", InstEmit.Usqadd_V,       InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0111111101xxxxxx000101xxxxxxxxxx", InstEmit.Usra_S,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x10111100>>>xxx000101xxxxxxxxxx", InstEmit.Usra_V,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0110111101xxxxxx000101xxxxxxxxxx", InstEmit.Usra_V,         InstEmit86.Und, typeof(OpCodeSimdShImm64));
            SetA64("0x101110<<1xxxxx001000xxxxxxxxxx", InstEmit.Usubl_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x101110<<1xxxxx001100xxxxxxxxxx", InstEmit.Usubw_V,        InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<0xxxxx000110xxxxxxxxxx", InstEmit.Uzp1_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<0xxxxx010110xxxxxxxxxx", InstEmit.Uzp2_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0x001110<<100001001010xxxxxxxxxx", InstEmit.Xtn_V,          InstEmit86.Und, typeof(OpCodeSimd64));
            SetA64("0>001110<<0xxxxx001110xxxxxxxxxx", InstEmit.Zip1_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
            SetA64("0>001110<<0xxxxx011110xxxxxxxxxx", InstEmit.Zip2_V,         InstEmit86.Und, typeof(OpCodeSimdReg64));
#endregion

#region "Generate InstA64FastLookup Table (AArch64)"
            var tmp = new List<InstInfo>[_fastLookupSize];
            for (int i = 0; i < _fastLookupSize; i++)
            {
                tmp[i] = new List<InstInfo>();
            }

            foreach (var inst in _allInstA64)
            {
                int mask  = ToFastLookupIndex(inst.Mask);
                int value = ToFastLookupIndex(inst.Value);

                for (int i = 0; i < _fastLookupSize; i++)
                {
                    if ((i & mask) == value)
                    {
                        tmp[i].Add(inst);
                    }
                }
            }

            for (int i = 0; i < _fastLookupSize; i++)
            {
                _instA64FastLookup[i] = tmp[i].ToArray();
            }
#endregion
        }

        private class InstInfo
        {
            public int Mask;
            public int Value;

            public Inst Inst;

            public InstInfo(int mask, int value, Inst inst)
            {
                Mask  = mask;
                Value = value;
                Inst  = inst;
            }
        }

        private static List<InstInfo> _allInstA32 = new List<InstInfo>();
        private static List<InstInfo> _allInstA64 = new List<InstInfo>();

        private static int _fastLookupSize = 0x1000;
        private static InstInfo[][] _instA64FastLookup = new InstInfo[_fastLookupSize][];

        private static void SetA32(string encoding, InstInterpreter interpreter, Type type)
        {
            Set(encoding, new Inst(interpreter, null, InstEmit86.Und, type), ExecutionMode.AArch32);
        }

        private static void SetA64(string encoding, InstEmitter emitter, InstEmitter86 emitter86, Type type)
        {
            Set(encoding, new Inst(null, emitter, emitter86, type), ExecutionMode.AArch64);
        }

        private static void Set(string encoding, Inst inst, ExecutionMode mode)
        {
            int bit   = encoding.Length - 1;
            int value = 0;
            int xMask = 0;
            int xBits = 0;

            int[] xPos = new int[encoding.Length];

            int blacklisted = 0;

            for (int index = 0; index < encoding.Length; index++, bit--)
            {
                //Note: < and > are used on special encodings.
                //The < means that we should never have ALL bits with the '<' set.
                //So, when the encoding has <<, it means that 00, 01, and 10 are valid,
                //but not 11. <<< is 000, 001, ..., 110 but NOT 111, and so on...
                //For >, the invalid value is zero. So, for >> 01, 10 and 11 are valid,
                //but 00 isn't.
                char chr = encoding[index];

                if (chr == '1')
                {
                    value |= 1 << bit;
                }
                else if (chr == 'x')
                {
                    xMask |= 1 << bit;
                }
                else if (chr == '>')
                {
                    xPos[xBits++] = bit;
                }
                else if (chr == '<')
                {
                    xPos[xBits++] = bit;

                    blacklisted |= 1 << bit;
                }
                else if (chr != '0')
                {
                    throw new ArgumentException(nameof(encoding));
                }
            }

            xMask = ~xMask;

            if (xBits == 0)
            {
                InsertInst(xMask, value, inst, mode);

                return;
            }

            for (int index = 0; index < (1 << xBits); index++)
            {
                int mask = 0;

                for (int x = 0; x < xBits; x++)
                {
                    mask |= ((index >> x) & 1) << xPos[x];
                }

                if (mask != blacklisted)
                {
                    InsertInst(xMask, value | mask, inst, mode);
                }
            }
        }

        private static void InsertInst(
            int           xMask,
            int           value,
            Inst          inst,
            ExecutionMode mode)
        {
            InstInfo info = new InstInfo(xMask, value, inst);

            if (mode == ExecutionMode.AArch64)
            {
                _allInstA64.Add(info);
            }
            else
            {
                _allInstA32.Add(info);
            }
        }

        public static Inst GetInstA32(int opCode)
        {
            return GetInstFromList(_allInstA32, opCode);
        }

        public static Inst GetInstA64(int opCode)
        {
            return GetInstFromList(_instA64FastLookup[ToFastLookupIndex(opCode)], opCode);
        }

        private static int ToFastLookupIndex(int value)
        {
            return ((value >> 10) & 0x00F) | ((value >> 18) & 0xFF0);
        }

        private static Inst GetInstFromList(IEnumerable<InstInfo> instList, int opCode)
        {
            foreach (var node in instList)
            {
                if ((opCode & node.Mask) == node.Value)
                {
                    return node.Inst;
                }
            }

            return Inst.Undefined;
        }
    }
}
