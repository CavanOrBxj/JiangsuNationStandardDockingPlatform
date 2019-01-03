﻿using System.Threading;
using System.Collections.Generic;
using System.Data;
using EmergencyBroadcastingDockingPlatform.model;

namespace EmergencyBroadcastingDockingPlatform
{
    public class SingletonInfo
    {
        private static SingletonInfo _singleton;


        public string Longitude;//经度
        public string Latitude;//纬度
       

        public string CheckEBMStatusFlag;
        public strategytactics audit;
        public string HeartbeatInterval;//主动发送心跳间隔
        public string PlatformInfoInterval;//主动发送平台信息间隔
        public string EBRDTInfoInterval;//主动上报终端信息间隔
        public string EBRDTStateInterval;//主动上报终端状态间隔
        public string PlatformStateInterval;//主动发送平台状态间隔
       
        public int SequenceCodes;//顺序码
        public bool IsFirstLoad;//是否首次登录系统  

        public int EBRPSStateCode;//对接系统告知上级平台本平台的状态
        public string RemoteFTPpath;//融合平台FTP路径
        public MQ m_mq;
        public MQ m_mq_checkEBM;
        public Dictionary<string, string> DicTsCmd_ID;
        public Dictionary<string, List<Thread>> DicPlayingThread;

        public string USER_PRIORITY;
        public string TsCmd_UserID;
        public string USER_ORG_CODE;

        public int TerminalCount;//数据库中的终端数量

        public object lockedHttpSend;

        public List<EBMBrdItems> EBMBrdItems;
        public Dictionary<string, OrganizationInfo> DicOrganizationCode;

        public string SendTarAddress;//发送tar包的地址  20181218
        public string CurrentURL;//当前平台对县平台的url
        public string CurrentResourcecode;//当前平台的23位资源码
        public string PlatformEBRName;//平台名称
        public string PlatformAddress;//平台地址
        public string PlatformContact;//平台联系人
        public string PlatformPhoneNumber;//联系人手机
        public string PlatformLongitude;//所在平台经度
        public string PlatformLatitude;//所在平台维度



        private SingletonInfo()                                                                 
        {
            Longitude = "";
            Latitude = "";
            CurrentURL = "";
            CheckEBMStatusFlag = "";
            audit = new strategytactics();
            audit.TimeList = new List<timestrategies>();
            HeartbeatInterval = "";
            PlatformInfoInterval = "";
            PlatformStateInterval = "";
            EBRDTInfoInterval = "";
            EBRDTStateInterval = "";
            SequenceCodes = 0;
            IsFirstLoad = false;
            EBRPSStateCode = 0;
            RemoteFTPpath = "";
            m_mq = null;
            m_mq_checkEBM = null;

            DicTsCmd_ID = new Dictionary<string, string>();
            DicPlayingThread = new Dictionary<string, List<Thread>>();

            SendTarAddress = "";
            CurrentResourcecode = "";
            PlatformEBRName = "";
            PlatformAddress = "";
            PlatformContact = "";
            PlatformPhoneNumber = "";
            PlatformLongitude = "";
            PlatformLatitude = "";

        }
        public static SingletonInfo GetInstance()
        {
            if (_singleton == null)
            {
                Interlocked.CompareExchange(ref _singleton, new SingletonInfo(), null);
            }
            return _singleton;
        }
    }
}