namespace EnoLandingPageCore.Messages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;


    /// <summary>
    /// A DTO representing General Information about the CTF.
    /// </summary>
    public class CtfInfoMessage
    {
        /// <summary>
        /// Create a new CtfInfoMessage
        /// </summary>
        /// <param name="title"></param>
        /// <param name="startTime"></param>
        /// <param name="registrationCloseOffset"></param>
        /// <param name="checkInBeginOffset"></param>
        /// <param name="checkInEndOffset"></param>
        public CtfInfoMessage(string title, DateTime startTime, DateTime endTime, DateTime registrationCloseOffset, DateTime checkInBeginOffset, DateTime checkInEndOffset)
        {
            this.Title = title;
            this.CtfStartTime = startTime;
            this.CtfEndTime = endTime;
            this.RegistrationCloseTime = registrationCloseOffset;
            this.CheckInBeginTime = checkInBeginOffset;
            this.CheckInEndTime = checkInEndOffset;
        }

        /// <summary>
        /// The Title of the CTF.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The StartTime of the CTF.
        /// </summary>
        [Required]
        public DateTime CtfStartTime { get; set; }

        /// <summary>
        /// The EndTime of the CTF.
        /// </summary>
        /// <value></value>
        [Required]
        public DateTime CtfEndTime { get; set; }

        /// <summary>
        /// The Time until Teams have to be registred.
        /// </summary>
        /// <value></value>
        [Required]
        public DateTime RegistrationCloseTime { get; set; }

        /// <summary>
        /// The Time when the checkin will be available. 
        /// </summary>
        /// <value></value>
        [Required]
        public DateTime CheckInBeginTime { get; set; }

        /// <summary>
        /// The Time until Teams have to be checked in.
        /// </summary>
        /// <value></value>
        [Required]
        public DateTime CheckInEndTime { get; set; }

    }
}
