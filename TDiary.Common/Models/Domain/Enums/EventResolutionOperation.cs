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
        /// Add locally, play and send to server
        /// </summary>
        Push,
        /// <summary>
        /// Undo and remove locally
        /// </summary>
        UndoAndRemove,
        /// <summary>
        /// Merge changes, play locally, push to server
        /// </summary>
        Merge,
        /// <summary>
        /// Remove from local, used when deleted in both
        /// </summary>
        Remove,
        /// <summary>
        /// Nothing is to be done with this event
        /// </summary>
        NoOp
    }
}
