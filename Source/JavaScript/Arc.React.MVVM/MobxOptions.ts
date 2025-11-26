// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

export interface MobxOptions {
    enforceActions?: "never" | "always" | "observed";
    computedRequiresReaction?: boolean;
    /**
     * Warn if you try to create to derivation / reactive context without accessing any observable.
     */
    reactionRequiresObservable?: boolean;
    /**
     * Warn if observables are accessed outside a reactive context
     */
    observableRequiresReaction?: boolean;
    isolateGlobalState?: boolean;
    disableErrorBoundaries?: boolean;
    safeDescriptors?: boolean;
    reactionScheduler?: (f: () => void) => void;
    useProxies?: "always" | "never" | "ifavailable";
}