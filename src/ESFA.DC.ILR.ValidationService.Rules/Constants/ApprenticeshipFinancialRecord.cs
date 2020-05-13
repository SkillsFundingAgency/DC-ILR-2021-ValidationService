namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class ApprenticeshipFinancialRecord
    {
        //public const string TotalTrainingPrice = "TNP1";
        //public const string TotalAssessmentPrice = "TNP2";
        //public const string ResidualTrainingPrice = "TNP3";
        //public const string ResidualAssessmentPrice = "TNP4";
        //public const string TrainingPayment = "PMR1";
        //public const string AssessmentPayment = "PMR2";
        //public const string EmployerPaymentReimbursedByProvider = "PMR3";

        public static class Types
        {
            public const string TotalNegotiatedPrice = "TNP";
            public const string PaymentRecord = "PMR";
        }

        public static class TotalNegotiatedPriceCodes
        {
            public const int TotalTrainingPrice = 1;
            public const int TotalAssessmentPrice = 2;
            public const int ResidualTrainingPrice = 3;
            public const int ResidualAssessmentPrice = 4;
        }

        public static class PaymentRecordCodes
        {
            public const int TrainingPayment = 1;
            public const int AssessmentPayment = 2;
            public const int EmployerPaymentReimbursedByProvider = 3;
        }
    }
}
