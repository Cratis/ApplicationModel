// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Subscription } from 'rxjs';
import { Constructor } from '@cratis/fundamentals';

/**
 * Represents a message handler.
 */
export type MessageHandler<T> = (message: T) => void;

/**
 * Defines a system for publishing and subscribing to messages.
 */
export abstract class IMessenger {

    /**
     * Publish a message.
     * @param {*} message Message to publish.
     */
    abstract publish<TMessage extends object>(message: TMessage): void;

    /**
     * Subscribe to a specific message type.
     * @param {MessageHandler} callback Callback that gets called when message arrives.
     */
    abstract subscribe<TMessage extends object>(type: Constructor<TMessage>, callback: MessageHandler<TMessage>): Subscription;
}
