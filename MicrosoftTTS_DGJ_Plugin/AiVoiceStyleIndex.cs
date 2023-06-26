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
            AiVoiceStyleIndex._.Add("affectionate", "亲切、深情的风格");
            AiVoiceStyleIndex._.Add("angry", "愤怒、生气的风格");
            AiVoiceStyleIndex._.Add("assistant", "助理");
            AiVoiceStyleIndex._.Add("business", "商务、专业的风格");
            AiVoiceStyleIndex._.Add("calm", "平静、镇定的风格");
            AiVoiceStyleIndex._.Add("chat", "随意、非正式的聊天风格");
            AiVoiceStyleIndex._.Add("cheerful", "欢快、令人高兴的风格");
            AiVoiceStyleIndex._.Add("customerservice", "客服");
            AiVoiceStyleIndex._.Add("depressed", "沮丧的");
            AiVoiceStyleIndex._.Add("disgruntled", "不满的");
            AiVoiceStyleIndex._.Add("embarrassed", "尴尬的");
            AiVoiceStyleIndex._.Add("empathetic", "富有同理心的风格");
            AiVoiceStyleIndex._.Add("emphatic", "强调、有力的风格");
            AiVoiceStyleIndex._.Add("envious", "羡慕的、嫉妒的");
            AiVoiceStyleIndex._.Add("excited", "兴奋、激动的风格");
            AiVoiceStyleIndex._.Add("fearful", "害怕的");
            AiVoiceStyleIndex._.Add("formal", "正式、庄重的风格");
            AiVoiceStyleIndex._.Add("friendly", "友好的");
            AiVoiceStyleIndex._.Add("gentle", "温柔的");
            AiVoiceStyleIndex._.Add("lyrical", "抒情的");
            AiVoiceStyleIndex._.Add("narration-professional", "专业的叙述");
            AiVoiceStyleIndex._.Add("narration-relaxed", "放松的叙述");
            AiVoiceStyleIndex._.Add("newscast", "新闻播报风格");
            AiVoiceStyleIndex._.Add("newscast-casual", "非正式的新闻播报");
            AiVoiceStyleIndex._.Add("poetry-reading", "朗诵诗歌的");
            AiVoiceStyleIndex._.Add("sad", "悲伤、沮丧的风格");
            AiVoiceStyleIndex._.Add("serious", "严肃的");
            AiVoiceStyleIndex._.Add("sports-commentary", "体育评论");
            AiVoiceStyleIndex._.Add("sports-commentary-excited", "激动的体育评论");
            AiVoiceStyleIndex._.Add("tired", "疲倦、困乏的风格");


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
