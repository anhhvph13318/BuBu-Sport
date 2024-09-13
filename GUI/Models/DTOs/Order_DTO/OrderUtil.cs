namespace GUI.Models.DTOs.Order_DTO
{
    public static class OrderUtil
    {
        public static IDictionary<int, string> OrderStepProcessing = new Dictionary<int, string>()
        {
            { 0, "Chờ xác nhận" },
            { 1, "Chuẩn bị hàng" },
            { 2, "Đang giao hàng" },
            { 7, "Hoàn thành" }
        };

        public static IDictionary<int, string> NextOrderStepProcessing(int currentStep)
        {
            var steps = new Dictionary<int, string>()
            {
                { currentStep, OrderStepProcessing[currentStep] }
            };

            switch(currentStep)
            {
                case 0:
                    steps.Add(1, OrderStepProcessing[1]);
                    break;
                case 1:
                    steps.Add(2, OrderStepProcessing[2]);
                    break;
                case 2:
                    steps.Add(7, OrderStepProcessing[7]);
                    break;
                default:
                    break;
            }

            return steps;
        }
    }
}
