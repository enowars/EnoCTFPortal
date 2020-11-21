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
            HetznerCloudApiCall call,
            CancellationToken token,
            TaskCompletionSource tcs,
            bool isScheduled)
        {
            this.Call = call;
            this.Token = token;
            this.Tcs = tcs;
            this.IsRunning = isScheduled;
        }

        public HetznerCloudApiCall Call { get; set; }

        public CancellationToken Token { get; set; }

        public TaskCompletionSource Tcs { get; set; }

        public bool IsRunning { get; set; }
    }
}
