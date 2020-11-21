namespace EnoLandingPageBackend.Hetzner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class HetznerCloudApiScheduledCall
    {
        public HetznerCloudApiScheduledCall(
            HetznerCloudApiCallType callType,
            TaskCompletionSource tcs,
            CancellationToken token)
        {
            this.CallType = callType;
            this.Token = token;
            this.Tcs = tcs;
            this.IsRunning = false;
        }

        public HetznerCloudApiCallType CallType { get; set; }

        public CancellationToken Token { get; set; }

        public TaskCompletionSource Tcs { get; set; }

        public bool IsRunning { get; set; }
    }
}
