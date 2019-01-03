using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmergencyBroadcastingDockingPlatform.AudioMessage;
using System.Data;
using System.IO;
using System.Xml;
using System.Drawing;

namespace EmergencyBroadcastingDockingPlatform.AudioMessage.MQAudio
{
    //播放状态反馈类
    public class AudioPlayState : IPlayState
    {
        private delegate bool HandlingDelegate(string TmcId, string path, string BrdStateCode);
        private event HandlingDelegate HandlingEvent;
        //未播放

        //播放中

        /// <summary>
        /// 播放完成
        /// </summary>
        /// <returns></returns>
        public bool NotPlay(string TmcId, string path, string BrdStateCode)
        {
            try
            {
                bool Radio= EmergencyBroadcast(TmcId, path, BrdStateCode,null);
                return Radio;
            }
            catch (Exception ex)
            {
                throw new Exception("未播放:" +ex.Message);
            }
            return false;

        }
        public bool FeedbackFunction(EBD ebdsr,string BrdStateCode, string TimingTerminalState)
        {
            bool flag = false;
            try
            {
                if (string.IsNullOrEmpty(TimingTerminalState))
                {
                    bool eb = sendEBMStateResponse(ebdsr, BrdStateCode);
                    if (eb)
                    {
                        flag = true;
                    }
                }
                else
                {
                    bool eb= sendEBMStateResponse(ebdsr, BrdStateCode);
                    bool Up = UpdateState(TimingTerminalState);
                    if (eb && Up)
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return flag;
        }
        private bool EmergencyBroadcast(string TmcId, string path, string BrdStateCode,string TimingTerminalState)
        {
            EBD ebd;
            DataTable dt;
            bool flag = false;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                    String xmlInfo = sr.ReadToEnd();
                    xmlInfo = xmlInfo.Replace("xmlns:xs", "xmlns");
                    sr.Close();
                    xmlInfo = XmlSerialize.ReplaceLowOrderASCIICharacters(xmlInfo);
                    xmlInfo = XmlSerialize.GetLowOrderASCIICharacters(xmlInfo);
                    ebd = XmlSerialize.DeserializeXML<EBD>(xmlInfo);
                }
                if (Convert.ToInt32(BrdStateCode) != 0)
                {
                    if (Convert.ToInt32(TmcId) < 0)
                    {
                        return flag;
                    }
                    else
                    {
                        dt = ViewDataTsCmdStore(TmcId);
                        if (dt != null && dt.Rows.Count > 0)
                        {

                            flag = FeedbackFunction(ebd, BrdStateCode, TimingTerminalState);
                            return flag;
                        }

                    }
                }
                else
                {

                    flag = FeedbackFunction(ebd, BrdStateCode, TimingTerminalState);
                    return flag;
                }
                return false;

            }
            catch (Exception ex)
            {
                throw new Exception("应急消息回馈:" + ex.Message);
            }
        }

        public string GetSequenceCodes()
        {
            SingletonInfo.GetInstance().SequenceCodes += 1;
            return SingletonInfo.GetInstance().SequenceCodes.ToString().PadLeft(16, '0');
        }

        /// <summary>
        /// 播发状态反馈  20181213
        /// </summary>
        /// <param name="ebdsr"></param>
        /// <param name="BrdStateDesc"></param>
        /// <param name="BrdStateCode"></param>
        /// <returns></returns>
        private bool sendEBMStateResponse(EBD ebdsr, string BrdStateCode)
        {
            //*反馈
            #region 先删除解压缩包中的文件

            bool flag = false;
            foreach (string xmlfiledel in Directory.GetFileSystemEntries(ServerForm.sEBMStateResponsePath))
            {
                if (File.Exists(xmlfiledel))
                {
                    FileInfo fi = new FileInfo(xmlfiledel);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(xmlfiledel);//直接删除其中的文件  
                }
            }
            #endregion End
            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            //rHeart.SourceAreaCode = ServerForm. strSourceAreaCode;
            //rHeart.SourceType = ServerForm.strSourceType;
            //rHeart.SourceName = ServerForm.strSourceName;
            //rHeart.SourceID = ServerForm.strSourceID;
            //rHeart.sHBRONO = SingletonInfo.GetInstance().CurrentResourcecode;
           
         
            
            string frdStateName = "10" + SingletonInfo.GetInstance().CurrentResourcecode + GetSequenceCodes();
            string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";
            xmlHeartDoc = rHeart.EBMStateRequestResponse(ebdsr, frdStateName,BrdStateCode);

            TarXml.AudioResponseXml.CreateXML(xmlHeartDoc, ServerForm.sEBMStateResponsePath + xmlEBMStateFileName);
            ServerForm.mainFrm.GenerateSignatureFile(ServerForm.sEBMStateResponsePath, frdStateName);
            ServerForm.tar.CreatTar(ServerForm.sEBMStateResponsePath, ServerForm.sSendTarPath, frdStateName);// "HB000000000001");//使用新TAR


            string sHeartBeatTarName = ServerForm.sSendTarPath + "\\EBDT_" + frdStateName + ".tar";
            try
            {
               string result=  HttpSendFile.UploadFilesByPost(SingletonInfo.GetInstance().SendTarAddress, sHeartBeatTarName);
                if (result != "0")
                {
                    return true;
                }
            }
            catch (Exception w)
            {
                Log.Instance.LogWrite("应急消息播发状态反馈发送平台错误：" + w.Message);
            }
            return flag;
        }

        private bool UpdateState(string TimingTerminalState)
        {
            bool flag = false;
            XmlDocument xmlHeartDoc = new XmlDocument();
            responseXML rHeart = new responseXML();
            rHeart.SourceAreaCode = ServerForm.strSourceAreaCode;
            rHeart.SourceType = ServerForm.strSourceType;
            rHeart.SourceName = ServerForm.strSourceName;
            rHeart.SourceID = ServerForm.strSourceID;
            rHeart.sHBRONO = SingletonInfo.GetInstance().CurrentResourcecode;
            string MediaSql = "";
            string strSRV_ID = "";
            string strSRV_CODE = "";
            ServerForm.DeleteFolder(ServerForm.sHeartSourceFilePath);//删除原有XML发送文件的文件夹下的XML
            string frdStateName = "";
            List<Device> lDev = new List<Device>();
            try
            {
                MediaSql = "select  SRV.SRV_ID,SRV.SRV_CODE,SRV_GOOGLE,SRV_PHYSICAL_CODE,srv_detail,SRV_LOGICAL_CODE_GB,SRV_MFT_DATE,updateDate,SRV_RMT_STATUS  FROM SRV  left join Srvtype on   SRV.DeviceTypeId= Srvtype .srv_id where  Srvtype.srv_id=1";
                DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);
                if (dtMedia != null && dtMedia.Rows.Count > 0)
                {
                    if (dtMedia.Rows.Count > 100)
                    {
                        int mod = dtMedia.Rows.Count / 100 + 1;
                        for (int i = 0; i < mod; i++)
                        {
                            for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                            {
                                Device DV = new Device();
                                DV.SRV_ID = dtMedia.Rows[idtM][0].ToString();
                                strSRV_CODE = dtMedia.Rows[idtM][1].ToString();
                                DV.DeviceID = dtMedia.Rows[idtM]["SRV_LOGICAL_CODE_GB"].ToString();//修改于20180819 把资源码换成23位
                                DV.DeviceName = dtMedia.Rows[idtM][4].ToString();
                                DV.Latitude = dtMedia.Rows[idtM][2].ToString().Split(',')[0];
                                DV.Longitude = dtMedia.Rows[idtM][2].ToString().Split(',')[1];
                                DV.Srv_Mft_Date = dtMedia.Rows[idtM]["SRV_MFT_DATE"].ToString();
                                DV.UpdateDate = dtMedia.Rows[idtM]["updateDate"].ToString();
                                DV.DeviceState = TimingTerminalState;
                                lDev.Add(DV);
                            }
                            frdStateName = "10" + rHeart.sHBRONO + GetSequenceCodes();
                            string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                            xmlHeartDoc = rHeart.DeviceStateResponse(lDev, frdStateName);
                            TarXml.AudioResponseXml. CreateXML(xmlHeartDoc, ServerForm.sHeartSourceFilePath + xmlEBMStateFileName);
                            ServerForm.mainFrm.GenerateSignatureFile(ServerForm.sHeartSourceFilePath, frdStateName);
                            ServerForm.tar.CreatTar(ServerForm.sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                            string sHeartBeatTarName = ServerForm.sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                           string result =SendTar.SendTarOrder.sendHelper.AddPostQueue (SingletonInfo.GetInstance().SendTarAddress, sHeartBeatTarName);
                            if (result == "1")
                            {
                                flag = true;
                            }
                        }
                    }
                    else
                    {
                        for (int idtM = 0; idtM < dtMedia.Rows.Count; idtM++)
                        {
                            Device DV = new Device();
                            DV.SRV_ID = dtMedia.Rows[idtM][0].ToString();
                            strSRV_CODE = dtMedia.Rows[idtM][1].ToString();
                            DV.DeviceID = dtMedia.Rows[idtM]["SRV_LOGICAL_CODE_GB"].ToString();

                            DV.DeviceName = dtMedia.Rows[idtM][4].ToString();

                            DV.Latitude = dtMedia.Rows[idtM][2].ToString().Split(',')[0];
                            DV.Longitude = dtMedia.Rows[idtM][2].ToString().Split(',')[1];
                            DV.Srv_Mft_Date = dtMedia.Rows[idtM]["SRV_MFT_DATE"].ToString();
                            DV.UpdateDate = dtMedia.Rows[idtM]["updateDate"].ToString();
                            DV.DeviceState = TimingTerminalState;
                            lDev.Add(DV);
                        }
                        Random rdState = new Random();
                        frdStateName = "10" + rHeart.sHBRONO + GetSequenceCodes();
                        string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                        xmlHeartDoc = rHeart.DeviceStateResponse(lDev, frdStateName);
                        TarXml.AudioResponseXml. CreateXML(xmlHeartDoc, ServerForm. sHeartSourceFilePath + xmlEBMStateFileName);
                        ServerForm.mainFrm.GenerateSignatureFile(ServerForm.sHeartSourceFilePath, frdStateName);
                        ServerForm.tar.CreatTar(ServerForm.sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                        string sHeartBeatTarName = ServerForm.sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                        string result = SendTar.SendTarOrder.sendHelper.AddPostQueue(SingletonInfo.GetInstance().SendTarAddress, sHeartBeatTarName);
                        if (result == "1")
                        {
                            flag = true;
                        }
                    }
                }
                else
                {
                    Random rdState = new Random();
                    frdStateName = "10" + rHeart.sHBRONO + GetSequenceCodes();
                    string xmlEBMStateFileName = "\\EBDB_" + frdStateName + ".xml";

                    xmlHeartDoc = rHeart.DeviceStateResponse(lDev, frdStateName);
                    TarXml.AudioResponseXml. CreateXML(xmlHeartDoc, ServerForm. sHeartSourceFilePath + xmlEBMStateFileName);
                    ServerForm.mainFrm.GenerateSignatureFile(ServerForm.sHeartSourceFilePath, frdStateName);
                    ServerForm.tar.CreatTar(ServerForm.sHeartSourceFilePath, ServerForm.sSendTarPath, frdStateName);//使用新TAR
                    string sHeartBeatTarName = ServerForm.sSendTarPath + "\\" + "EBDT_" + frdStateName + ".tar";
                    string result = SendTar.SendTarOrder.sendHelper.AddPostQueue(SingletonInfo.GetInstance().SendTarAddress, sHeartBeatTarName);
                    if (result == "1")
                    {
                        flag = true;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("终端状态变更:" + ex.Message);
            }
            return flag;
        }

        public DataTable ViewDataTsCmdStore(string TsCmd_ID)
        {
            string MediaSql;
            try
            {

                MediaSql = "select TsCmd_ID,TsCmd_ExCute from  TsCmdStore where TsCmd_ID='" + TsCmd_ID + "'";
                //  MediaSql = "select top(1)TsCmd_ID,TsCmd_XmlFile from  TsCmdStore where TsCmd_ValueID = '" + ebd.EBMStateRequest.EBM.EBMID + "' order by TsCmd_Date desc";
                DataTable dtMedia = mainForm.dba.getQueryInfoBySQL(MediaSql);

                return dtMedia != null && dtMedia.Rows.Count > 0 ? dtMedia : null;
            }
            catch (Exception ex)
            {
               // throw new Exception("查询TsCmdStore出现异常:" + ex.Message);
                return null;
            }
          
        }

        public bool Playing(string TmcId, string path, string BrdStateCode,string TimingTerminalState)
        {
            try
            {
                bool Radio = EmergencyBroadcast(TmcId, path, BrdStateCode, TimingTerminalState);
                return Radio;
            }
            catch (Exception ex)
            {
               //throw new Exception("未播放:" + ex.Message);
                return false;
            }
           
        }

        public bool PlayOver(string TmcId, string path, string BrdStateCode, string TimingTerminalState)
        {
            try
            {
                bool Radio = EmergencyBroadcast(TmcId, path, BrdStateCode, TimingTerminalState);
                return Radio;
            }
            catch (Exception ex)
            {
               // throw new Exception("未播放:" + ex.Message);
                return false;
            }
          
        }

        public bool Untreated(string path, string BrdStateCode)
        {
            try
            {
                bool Radio = EmergencyBroadcast("-1", path, BrdStateCode,null);
                return Radio;
            }
            catch (Exception ex)
            {
               // throw new Exception("未播放:" + ex.Message);
                return false;
            }
          
        }
    }
}
