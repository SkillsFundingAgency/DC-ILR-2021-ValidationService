using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Structs
{
    public struct R68AppFinRecord
    {
        public R68AppFinRecord(int aimSeqNumber, int? fworkCode, int? stdCode, string aFinType, int aFinCode, DateTime aFinDate)
        {
            AimSeqNumber = aimSeqNumber;
            FworkCode = fworkCode;
            StdCode = stdCode;
            AFinType = aFinType;
            AFinCode = aFinCode;
            AFinDate = aFinDate;
        }

        public int AimSeqNumber { get; set; }

        public int? FworkCode { get; set; }

        public int? StdCode { get; set; }

        public string AFinType { get; set; }

        public int AFinCode { get; set; }

        public DateTime AFinDate { get; set; }
    }
}
