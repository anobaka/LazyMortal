using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Bootstrap.Components.IM.DingTalk.Models.ResponseModels
{
    public class CorpConversationMessageSendRequestModel
    {
        /// <summary>
        /// Do not set this property manually.
        /// </summary>
        [Required] [JsonProperty("agent_id")] public string AgentId { get; set; }

        private List<string> _userIds;

        [JsonIgnore]
        public List<string> UserIds
        {
            get => _userIds;
            set
            {
                _userIds = value;
                UserIdList = value == null ? null : string.Join(',', value);
            }
        }

        /// <summary>
        /// This property is used for request. Use <see cref="UserIds"/> on programming.
        /// </summary>
        [JsonProperty("userid_list")]
        public string UserIdList { get; set; }

        private List<string> _deptIds;

        public List<string> DeptIds
        {
            get => _deptIds;
            set
            {
                _deptIds = value;
                DeptIdList = value == null ? null : string.Join(',', value);
            }
        }

        /// <summary>
        /// This property is used for request. Use <see cref="DeptIds"/> on programming.
        /// </summary>
        [JsonProperty("dept_id_list")]
        public string DeptIdList { get; set; }

        [JsonProperty("to_all_user")] public bool? ToAllUser { get; set; }
        [JsonProperty("msg")] public DingTalkMessage Message { get; set; }
    }
}