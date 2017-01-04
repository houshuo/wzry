namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class ApolloPerson : ApolloStruct<ApolloPerson>
    {
        public override ApolloPerson FromString(string src)
        {
            char[] separator = new char[] { '&' };
            foreach (string str in src.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
                char[] chArray2 = new char[] { '=' };
                string[] strArray3 = str.Split(chArray2);
                if (strArray3.Length > 1)
                {
                    if (strArray3[0].CompareTo("NickName") == 0)
                    {
                        this.NickName = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("OpenId") == 0)
                    {
                        this.OpenId = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("Gender") == 0)
                    {
                        this.Gender = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("PictureSmall") == 0)
                    {
                        this.PictureSmall = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("PictureMiddle") == 0)
                    {
                        this.PictureMiddle = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("PictureLarge") == 0)
                    {
                        this.PictureLarge = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("Provice") == 0)
                    {
                        this.Provice = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("City") == 0)
                    {
                        this.City = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("IsFriend") == 0)
                    {
                        this.IsFriend = Convert.ToBoolean(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("Distance") == 0)
                    {
                        this.Distance = int.Parse(strArray3[1]);
                    }
                    else if (strArray3[0].CompareTo("Lang") == 0)
                    {
                        this.Lang = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("Country") == 0)
                    {
                        this.Country = strArray3[1];
                    }
                    else if (strArray3[0].CompareTo("GpsCity") == 0)
                    {
                        this.GpsCity = strArray3[1];
                    }
                }
            }
            return this;
        }

        public string City { get; set; }

        public string Country { get; set; }

        public int Distance { get; set; }

        public string Gender { get; set; }

        public string GpsCity { get; set; }

        public bool IsFriend { get; set; }

        public string Lang { get; set; }

        public string NickName { get; set; }

        public string OpenId { get; set; }

        public string PictureLarge { get; set; }

        public string PictureMiddle { get; set; }

        public string PictureSmall { get; set; }

        public string Provice { get; set; }
    }
}

