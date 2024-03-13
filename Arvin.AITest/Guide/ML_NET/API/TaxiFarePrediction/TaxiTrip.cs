using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Arvin.AITest.Guide.ML_NET.API.TaxiFarePrediction
{
    public class TaxiTrip
    {
        /// <summary>
        /// 【特征】出租车供应商的 ID
        /// </summary>
        [LoadColumn(0)]
        public string? VendorId;

        /// <summary>
        /// 【特征】出租车行程的费率类型
        /// </summary>
        [LoadColumn(1)]
        public string? RateCode;

        /// <summary>
        /// 【特征】行程中的乘客人数
        /// </summary>
        [LoadColumn(2)]
        public float PassengerCount;

        /// <summary>
        /// 行程所花的时间，希望在行程完成前预测行程费用。 当时并不知道行程有多长。 因此，行程时间不是一项特征，需要从模型删除此列
        /// </summary>
        [LoadColumn(3)]
        public float TripTime;
        /// <summary>
        /// 【特征】行程距离
        /// </summary>
        [LoadColumn(4)]
        public float TripDistance;
        /// <summary>
        /// 【特征】付款方式（现金或信用卡）
        /// </summary>
        [LoadColumn(5)]
        public string? PaymentType;
        /// <summary>
        /// 【特征】支付的总出租车费用
        /// </summary>
        [LoadColumn(6)]
        public float FareAmount;
    }

    public class TaxiTripFarePrediction
    {
        /// <summary>
        /// 预测结果，对于Score，包含了预测的Label值
        /// </summary>
        [ColumnName("Score")]
        public float FareAmount;
    }
}
