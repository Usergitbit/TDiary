using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Common.Models.Domain.Enums
{
    public enum EventResolutionOperation
    {
        /// <summary>
        /// Add locally and play
        /// </summary>
        Pull,

        /// <summary>
        /// Send to server
        /// </summary>
        Push,

        /// <summary>
        /// Undo and remove locally
        /// </summary>
        Undo,

        /// <summary>
        /// Merge changes, play locally, push to server
        /// </summary>
        Merge,

        /// <summary>
        /// Nothing is to be done with this event
        /// </summary>
        None,

        /// <summary>
        /// Validate relationships, add locally, play and send to server
        /// </summary>
        PushIfValid,
    }
}
