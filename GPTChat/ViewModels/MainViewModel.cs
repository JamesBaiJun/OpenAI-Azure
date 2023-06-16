using DevExpress.Mvvm.DataAnnotations;
using GPTChat.Models;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Wpf.Ui.Controls;
using System.Diagnostics;

namespace GPTChat.ViewModels
{
    [POCOViewModel]
    public class MainViewModel
    {
        public MainViewModel()
        {
            AzureKey = ConfigHelper.GetConfig("Azure");
            AzureRegion = ConfigHelper.GetConfig("AzureRegion");
            OpenAIKey = ConfigHelper.GetConfig("OpenAI");

            GPTRequest.OnReceiveReply += GPTRequest_OnReceiveReply;
            GPTRequest.OnReceivePartial += GPTRequest_OnReceivePartial;
            InitVoiceService();
        }

        public virtual ObservableCollection<Message> MessageItems { get; set; } = new ObservableCollection<Message>();
        public virtual bool VoiceIsEnable { get; set; } = true;
        public virtual string ResponsePartial { get; set; }
        public virtual string InputMessage { get; set; }
        public virtual bool IsWaitting { get; set; }
        public virtual bool IsSystem { get; set; }
        public virtual string OpenAIKey { get; set; }
        public virtual string AzureKey { get; set; }
        public virtual string AzureRegion { get; set; }

        /// <summary>
        /// 是否正在语音输入
        /// </summary>
        public virtual bool IsVocieInputting { get; set; }

        /// <summary>
        /// 可用的声音列表
        /// </summary>
        public virtual Dictionary<string, string> VoiceNameList { get; set; }

        /// <summary>
        /// 当前选择的声音名称
        /// </summary>
        public virtual string SelectedVoiceName { get; set; }

        #region Chat
        private void GPTRequest_OnReceivePartial(string obj)
        {
            ResponsePartial += obj;
        }

        SpeechSynthesizer speech;

        private async void GPTRequest_OnReceiveReply(string obj)
        {
            MessageItems.Add(new Message(Role.Assistant, obj));
            IsWaitting = false;
            ResponsePartial = string.Empty;
            await speech.StopSpeakingAsync();
            if (VoiceIsEnable)
            {
                await speech.SpeakTextAsync(obj);
            }
        }

        public async void SendMessage()
        {
            try
            {
                MessageItems.Add(new Message(IsSystem ? Role.System : Role.User, InputMessage));
                InputMessage = string.Empty;
                IsWaitting = true;

                await GPTRequest.AskAsync(MessageItems.ToList());
                IsWaitting = false;
            }
            catch (Exception e)
            {
                MessageBox message = new MessageBox
                {
                    Title = "错误",
                    Content = e.Message,
                    ButtonLeftName = "OK",
                    ButtonRightName = "Cancel",
                };

                message.ButtonLeftClick += (s, e) => message.Close();
                message.ButtonRightClick += (s, e) => message.Close();
                message.ShowDialog();
            }
            finally
            {
                IsWaitting = false;
            }
        }
        #endregion

        #region 文本朗读
        private async void InitVoiceService()
        {
            var config = SpeechConfig.FromSubscription(AzureKey, AzureRegion);
            config.SpeechSynthesisVoiceName = "zh-CN-XiaoxiaoNeural";
            speech = new SpeechSynthesizer(config);

            var allVocies = await speech.GetVoicesAsync("zh-CN");
            var listDic = new Dictionary<string, string>();
            foreach (var item in allVocies.Voices)
            {
                listDic[item.LocalName] = item.ShortName;
            }
            VoiceNameList = listDic;
        }

        public void OnSelectedVoiceNameChanged()
        {
            if (string.IsNullOrEmpty(SelectedVoiceName))
            {
                return;
            }

            var config = SpeechConfig.FromSubscription(AzureKey, AzureRegion);
            config.SpeechSynthesisVoiceName = SelectedVoiceName;
            speech = new SpeechSynthesizer(config);
        }

        public async void OnVoiceIsEnableChanged()
        {
            if (!VoiceIsEnable)
            {
                await speech.StopSpeakingAsync();
            }
        }

        #endregion

        #region 语音输入

        public async void OnIsVocieInputtingChanged()
        {
            if (IsVocieInputting)
            {
                SpeechConfig config = SpeechConfig.FromSubscription(AzureKey, AzureRegion);
                config.SpeechRecognitionLanguage = "zh-cn";
                using (var recognizer = new SpeechRecognizer(config))
                {
                    var result = await recognizer.RecognizeOnceAsync();
                    if (result.Reason == ResultReason.RecognizedSpeech)
                    {
                        InputMessage += result.Text;
                    }

                    IsVocieInputting = false;
                }
            }
        }
        #endregion

        public void Apply()
        {
            InitVoiceService();
            GPTRequest.SetKey(OpenAIKey);
            ConfigHelper.SaveConfig("Azure", AzureKey);
            ConfigHelper.SaveConfig("AzureRegion", AzureRegion);
            ConfigHelper.SaveConfig("OpenAI", OpenAIKey);
        }
    }
}
