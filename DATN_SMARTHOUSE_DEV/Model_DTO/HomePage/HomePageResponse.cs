namespace DATN_ACV_DEV.Model_DTO.HomePage
{
    public class HomePageResponse
    {

        public HomePageResponse()
        {
            LstProduct = new List<HomePageModel>();
        }
        public List<HomePageModel> LstProduct { get; set; }
        public int TotalCount { get; set; }
    }
}
