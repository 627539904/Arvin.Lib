using System;
using System.Collections.Generic;
using System.Text;

namespace Arvin.Helpers
{
    /// <summary>
    /// 身份证验证
    /// </summary>
    public class IDCardHelper
    {
        public static bool CheckIDCard(string IDCard)
        {
            if (IDCard.Length == 15)
            {
                if (CheckIDCard15(IDCard) == true)
                {
                    return true;
                }
            }
            if (IDCard.Length == 18)
            {
                if (CheckIDCard18(IDCard) == true)
                {
                    return true;
                }
            }
            return false;
        }

        // <summary>
        /// 验证15位身份证号
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>验证成功为True，否则为False</returns>
        public static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }

        /// <summary>
        /// 验证18位身份证号
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>验证成功为True，否则为False</returns>
        public static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        public static string GetBirthFromIDCard(string IDCard)
        {
            if (IDCard.Length == 15)
            {
                IDCard = IDCard15To18(IDCard);
            }
            string Birth = IDCard.Substring(6, 8);
            return Birth;
        }

        /// <summary>
        /// 转换15位身份证号码为18位
        /// </summary>
        /// <param name="oldIDCard">15位的身份证</param>
        /// <returns>返回18位的身份证</returns>
        private static string IDCard15To18(string oldIDCard)
        {
            int iS = 0;

            //加权因子常数
            int[] iW = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            //校验码常数
            string LastCode = "10X98765432";
            //新身份证号
            string newIDCard;

            newIDCard = oldIDCard.Substring(0, 6);
            //填在第6位及第7位上填上‘1’，‘9’两个数字
            newIDCard += "19";

            newIDCard += oldIDCard.Substring(6, 9);

            //进行加权求和
            for (int i = 0; i < 17; i++)
            {
                iS += int.Parse(newIDCard.Substring(i, 1)) * iW[i];
            }

            //取模运算，得到模值
            int iY = iS % 11;
            //从LastCode中取得以模为索引号的值，加到身份证的最后一位，即为新身份证号。
            newIDCard += LastCode.Substring(iY, 1);
            return newIDCard;
        }

        public static string GetSex(string IDCard)
        {
            string Sex = "";
            if (IDCard.Length == 18)
            {
                int PP = Convert.ToInt32(IDCard.Substring(16, 1)) % 2;//判断倒数第二位是奇数还是偶数
                if (PP == 0)//如果是偶数
                {
                    Sex = "女";//说明身份证号码的持有者是女性
                }
                else
                {
                    Sex = "男";//如果是奇数则身份证号码的持有者是男性
                }
            }
            else
            {
                int PP = Convert.ToInt32(IDCard.Substring(14, 1)) % 2;//判断倒数第二位是奇数还是偶数
                if (PP == 0)//如果是偶数
                {
                    Sex = "女";//说明身份证号码的持有者是女性
                }
                else
                {
                    Sex = "男";//如果是奇数则身份证号码的持有者是男性
                }
            }
            return Sex;
        }
    }
}
