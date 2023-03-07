namespace Mandara.Business.Services.Prices
{
    public class Legs
    {
        public LegData Leg1Data { get; private set; }
        public LegData Leg2Data { get; private set; }

        public Legs(LegData leg1, LegData leg2)
        {
            Leg1Data = leg1;
            Leg2Data = leg2;
        }

        public static Legs Default = new Legs(LegData.GetDefault(), LegData.GetDefault());
    }
}