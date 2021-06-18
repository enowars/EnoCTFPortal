namespace EnoLandingPageCore.Messages
{
    using System;
    using System.Collections.Generic;
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
        public CtfInfoMessage(string title, DateTime startTime, DateTime registrationCloseOffset, DateTime checkInBeginOffset, DateTime checkInEndOffset)
        {
            this.Title = title;
            this.StartTime = startTime;
            this.RegistrationCloseOffset = registrationCloseOffset;
            this.CheckInBeginOffset = checkInBeginOffset;
            this.CheckInEndOffset = checkInEndOffset;
        }
        /// <summary>
        /// The Title of the CTF.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The StartTime of the CTF.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The Time until Teams have to be registred.
        /// </summary>
        /// <value></value>
        public DateTime RegistrationCloseOffset {get; set; }

        /// <summary>
        /// The Time when the checkin will be available. 
        /// </summary>
        /// <value></value>
        public DateTime CheckInBeginOffset {get; set; }

        /// <summary>
        /// The Time until Teams have to be checked in.
        /// </summary>
        /// <value></value>
        public DateTime CheckInEndOffset {get; set; }

    }
}
