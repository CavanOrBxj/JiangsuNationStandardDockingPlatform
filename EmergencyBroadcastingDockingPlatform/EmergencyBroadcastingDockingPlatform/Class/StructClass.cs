using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencyBroadcastingDockingPlatform
{

    public class timestrategies
    {
        /// <summary>
        /// 策略ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 策略开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 策略结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string EvenType { get; set; }
    }


    public class strategytactics
    {
        public List<timestrategies> TimeList; 
    }


    public class PlayElements
    {

        public EBD EBDITEM { get; set; }
        public string sAnalysisFileName { get; set; }
        public string xmlFilePath { get; set; }


        public string targetPath { get; set; }

        /// <summary>
        /// 数据表EBMInfo的ID
        /// </summary>
        public string EBMInfoID { get; set; }
    }

    public class OrganizationInfo
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string ORG_DETAIL { get; set; }

        /// <summary>
        /// 区域码
        /// </summary>
        public string GB_CODE { get; set; }
    }


}
