using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftTTS_DGJ_Plugin
{
    public class AiVoiceStyleIndex
    {
        private static Dictionary<string, string> _ = new Dictionary<string, string>();
        static AiVoiceStyleIndex()
        {
            AiVoiceStyleIndex._.Add("neutral", "中性风格");
            AiVoiceStyleIndex._.Add("chat", "随意、非正式的聊天风格");
            AiVoiceStyleIndex._.Add("newscast", "新闻播报风格");
            AiVoiceStyleIndex._.Add("cheerful", "欢快、令人高兴的风格");
            AiVoiceStyleIndex._.Add("affectionate", "亲切、深情的风格");
            AiVoiceStyleIndex._.Add("business", "商务、专业的风格");
            AiVoiceStyleIndex._.Add("empathetic", "富有同理心的风格");
            AiVoiceStyleIndex._.Add("excited", "兴奋、激动的风格");
            AiVoiceStyleIndex._.Add("formal", "正式、庄重的风格");
            AiVoiceStyleIndex._.Add("angry", "愤怒、生气的风格");
            AiVoiceStyleIndex._.Add("calm", "平静、镇定的风格");
            AiVoiceStyleIndex._.Add("sad", "悲伤、沮丧的风格");
            AiVoiceStyleIndex._.Add("tired", "疲倦、困乏的风格");
            AiVoiceStyleIndex._.Add("emphatic", "强调、有力的风格");
        }
        public static string GetName(string key)
        {
            if (AiVoiceStyleIndex._.ContainsKey(key))
            {
                return AiVoiceStyleIndex._[key];
            }
            return key;
        }
    }
}
