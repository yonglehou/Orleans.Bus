﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Orleans.Bus
{
    /// <summary>
    /// Base class for all kinds of message based grains
    /// </summary>
    public abstract class MessageBasedGrain : GrainBase, IGrain, IGrainInstance
    {
        public IGrainRuntime Runtime = GrainRuntime.Instance;
        public IMessageBus Bus = MessageBus.Instance;
        
        public IGrainInstance Instance;
        protected MessageBasedGrain()
        {
            Instance = this;
        }

        #if DEBUG
        
        /// <summary>
        /// Registers a timer to send periodic callbacks to this grain.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// This timer will not prevent the current grain from being deactivated.
        ///             If the grain is deactivated, then the timer will be discarded.
        /// 
        /// </para>
        /// 
        /// <para>
        /// Until the Task returned from the asyncCallback is resolved,
        ///             the next timer tick will not be scheduled.
        ///             That is to say, timer callbacks never interleave their turns.
        /// 
        /// </para>
        /// 
        /// <para>
        /// The timer may be stopped at any time by calling the <c>Dispose</c> method
        ///             on the timer handle returned from this call.
        /// 
        /// </para>
        /// 
        /// <para>
        /// Any exceptions thrown by or faulted Task's returned from the asyncCallback
        ///             will be logged, but will not prevent the next timer tick from being queued.
        /// 
        /// </para>
        /// 
        /// </remarks>
        /// <param name="asyncCallback">Callback function to be invoked when timr ticks.</param>
        /// <param name="state">State object that will be passed as argument when calling the asyncCallback.</param>
        /// <param name="dueTime">Due time for first timer tick.</param>
        /// <param name="period">Period of subsequent timer ticks.</param>
        /// <returns>
        /// Handle for this Timer.
        /// </returns>
        /// <seealso cref="T:Orleans.IOrleansTimer"/>
        protected new IOrleansTimer RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
        {
           return Instance.RegisterTimer(asyncCallback, state, dueTime, period);
        }

        /// <summary>
        /// Registers a persistent, reliable reminder to send regular notifications (reminders) to the grain.
        ///             The grain must implement the <c>Orleans.IRemindable</c> interface, and reminders for this grain will be sent to the <c>ReceiveReminder</c> callback method.
        ///             If the current grain is deactivated when the timer fires, a new activation of this grain will be created to receive this reminder.
        ///             If an existing reminder with the same name already exists, that reminder will be overwritten with this new reminder.
        ///             Reminders will always be received by one activation of this grain, even if multiple activations exist for this grain.
        /// 
        /// </summary>
        /// <param name="reminderName">Name of this reminder</param>
        /// <param name="dueTime">Due time for this reminder</param>
        /// <param name="period">Frequence period for this reminder</param>
        /// <returns>
        /// Promise for Reminder handle.
        /// </returns>
        protected new Task<IOrleansReminder> RegisterOrUpdateReminder(string reminderName, TimeSpan dueTime, TimeSpan period)
        {
            return Instance.RegisterOrUpdateReminder(reminderName, dueTime, period);
        }

        /// <summary>
        /// Unregisters a previously registered reminder.
        /// 
        /// </summary>
        /// <param name="reminder">Reminder to unregister.</param>
        /// <returns>
        /// Completion promise for this operation.
        /// </returns>
        protected new Task UnregisterReminder(IOrleansReminder reminder)
        {
            return Instance.UnregisterReminder(reminder);
        }

        /// <summary>
        /// Returns a previously registered reminder.
        /// 
        /// </summary>
        /// <param name="reminderName">Reminder to return</param>
        /// <returns>
        /// Promise for Reminder handle.
        /// </returns>
        protected new Task<IOrleansReminder> GetReminder(string reminderName)
        {
            return Instance.GetReminder(reminderName);
        }

        /// <summary>
        /// Returns a list of all reminders registered by the grain.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// Promise for list of Reminders registered for this grain.
        /// </returns>
        protected new Task<List<IOrleansReminder>> GetReminders()
        {
            return Instance.GetReminders();
        }

        /// <summary>
        /// Deactivate this activation of the grain after the current grain method call is completed.
        ///             This call will mark this activation of the current grain to be deactivated and removed at the end of the current method.
        ///             The next call to this grain will result in a different activation to be used, which typical means a new activation will be created automatically by the runtime.
        /// 
        /// </summary>
        protected new void DeactivateOnIdle()
        {
            Instance.DeactivateOnIdle();
        }

        /// <summary>
        /// Delay Deactivation of this activation at least for the specified time duration.
        ///             A positive <c>timeSpan</c> value means “prevent GC of this activation for that time span”.
        ///             A negative <c>timeSpan</c> value means “unlock, and make this activation available for GC again”.
        ///             DeactivateOnIdle method would undo / override any current “keep alive” setting,
        ///             making this grain immediately available  for deactivation.
        /// 
        /// </summary>
        protected new void DelayDeactivation(TimeSpan timeSpan)
        {
            Instance.DelayDeactivation(timeSpan);
        }

        #endif

        #region IGrainInstance

        IOrleansTimer IGrainInstance.RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
        {
            return base.RegisterTimer(asyncCallback, state, dueTime, period);
        }

        Task<IOrleansReminder> IGrainInstance.RegisterOrUpdateReminder(string reminderName, TimeSpan dueTime, TimeSpan period)
        {
            return base.RegisterOrUpdateReminder(reminderName, dueTime, period);
        }

        Task IGrainInstance.UnregisterReminder(IOrleansReminder reminder)
        {
            return base.UnregisterReminder(reminder);
        }

        Task<IOrleansReminder> IGrainInstance.GetReminder(string reminderName)
        {
            return base.GetReminder(reminderName);
        }

        Task<List<IOrleansReminder>> IGrainInstance.GetReminders()
        {
            return base.GetReminders();
        }

        void IGrainInstance.DeactivateOnIdle()
        {
            base.DeactivateOnIdle();
        }

        void IGrainInstance.DelayDeactivation(TimeSpan timeSpan)
        {
            base.DelayDeactivation(timeSpan);
        }

        #endregion
    }

    /// <summary>
    /// Base class for all kinds of persistent message based grains
    /// </summary>
    public abstract class MessageBasedGrain<TState> : GrainBase<TState>, IGrain, IGrainInstance 
        where TState : class, IGrainState
    {
        public IGrainRuntime Runtime = GrainRuntime.Instance;
        public IMessageBus Bus = MessageBus.Instance;

        public IGrainInstance Instance;
        protected MessageBasedGrain()
        {
            Instance = this;
        }

        #if DEBUG

        /// <summary>
        /// Registers a timer to send periodic callbacks to this grain.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// This timer will not prevent the current grain from being deactivated.
        ///             If the grain is deactivated, then the timer will be discarded.
        /// 
        /// </para>
        /// 
        /// <para>
        /// Until the Task returned from the asyncCallback is resolved,
        ///             the next timer tick will not be scheduled.
        ///             That is to say, timer callbacks never interleave their turns.
        /// 
        /// </para>
        /// 
        /// <para>
        /// The timer may be stopped at any time by calling the <c>Dispose</c> method
        ///             on the timer handle returned from this call.
        /// 
        /// </para>
        /// 
        /// <para>
        /// Any exceptions thrown by or faulted Task's returned from the asyncCallback
        ///             will be logged, but will not prevent the next timer tick from being queued.
        /// 
        /// </para>
        /// 
        /// </remarks>
        /// <param name="asyncCallback">Callback function to be invoked when timr ticks.</param>
        /// <param name="state">State object that will be passed as argument when calling the asyncCallback.</param>
        /// <param name="dueTime">Due time for first timer tick.</param>
        /// <param name="period">Period of subsequent timer ticks.</param>
        /// <returns>
        /// Handle for this Timer.
        /// </returns>
        /// <seealso cref="T:Orleans.IOrleansTimer"/>
        protected new IOrleansTimer RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
        {
            return Instance.RegisterTimer(asyncCallback, state, dueTime, period);
        }

        /// <summary>
        /// Registers a persistent, reliable reminder to send regular notifications (reminders) to the grain.
        ///             The grain must implement the <c>Orleans.IRemindable</c> interface, and reminders for this grain will be sent to the <c>ReceiveReminder</c> callback method.
        ///             If the current grain is deactivated when the timer fires, a new activation of this grain will be created to receive this reminder.
        ///             If an existing reminder with the same name already exists, that reminder will be overwritten with this new reminder.
        ///             Reminders will always be received by one activation of this grain, even if multiple activations exist for this grain.
        /// 
        /// </summary>
        /// <param name="reminderName">Name of this reminder</param>
        /// <param name="dueTime">Due time for this reminder</param>
        /// <param name="period">Frequence period for this reminder</param>
        /// <returns>
        /// Promise for Reminder handle.
        /// </returns>
        protected new Task<IOrleansReminder> RegisterOrUpdateReminder(string reminderName, TimeSpan dueTime, TimeSpan period)
        {
            return Instance.RegisterOrUpdateReminder(reminderName, dueTime, period);
        }

        /// <summary>
        /// Unregisters a previously registered reminder.
        /// 
        /// </summary>
        /// <param name="reminder">Reminder to unregister.</param>
        /// <returns>
        /// Completion promise for this operation.
        /// </returns>
        protected new Task UnregisterReminder(IOrleansReminder reminder)
        {
            return Instance.UnregisterReminder(reminder);
        }

        /// <summary>
        /// Returns a previously registered reminder.
        /// 
        /// </summary>
        /// <param name="reminderName">Reminder to return</param>
        /// <returns>
        /// Promise for Reminder handle.
        /// </returns>
        protected new Task<IOrleansReminder> GetReminder(string reminderName)
        {
            return Instance.GetReminder(reminderName);
        }

        /// <summary>
        /// Returns a list of all reminders registered by the grain.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// Promise for list of Reminders registered for this grain.
        /// </returns>
        protected new Task<List<IOrleansReminder>> GetReminders()
        {
            return Instance.GetReminders();
        }

        /// <summary>
        /// Deactivate this activation of the grain after the current grain method call is completed.
        ///             This call will mark this activation of the current grain to be deactivated and removed at the end of the current method.
        ///             The next call to this grain will result in a different activation to be used, which typical means a new activation will be created automatically by the runtime.
        /// 
        /// </summary>
        protected new void DeactivateOnIdle()
        {
            Instance.DeactivateOnIdle();
        }

        /// <summary>
        /// Delay Deactivation of this activation at least for the specified time duration.
        ///             A positive <c>timeSpan</c> value means “prevent GC of this activation for that time span”.
        ///             A negative <c>timeSpan</c> value means “unlock, and make this activation available for GC again”.
        ///             DeactivateOnIdle method would undo / override any current “keep alive” setting,
        ///             making this grain immediately available  for deactivation.
        /// 
        /// </summary>
        protected new void DelayDeactivation(TimeSpan timeSpan)
        {
            Instance.DelayDeactivation(timeSpan);
        }

        #endif

        TState explicitState;

        /// <summary>
        /// Gets or sets grain's state
        /// </summary>
        /// <remarks>You can use setter for testing purposes</remarks>
        public new TState State
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return explicitState ?? base.State; }
            set { explicitState = value; }
        }

        #region IGrainInstance

        IOrleansTimer IGrainInstance.RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
        {
            return base.RegisterTimer(asyncCallback, state, dueTime, period);
        }

        Task<IOrleansReminder> IGrainInstance.RegisterOrUpdateReminder(string reminderName, TimeSpan dueTime, TimeSpan period)
        {
            return base.RegisterOrUpdateReminder(reminderName, dueTime, period);
        }

        Task IGrainInstance.UnregisterReminder(IOrleansReminder reminder)
        {
            return base.UnregisterReminder(reminder);
        }

        Task<IOrleansReminder> IGrainInstance.GetReminder(string reminderName)
        {
            return base.GetReminder(reminderName);
        }

        Task<List<IOrleansReminder>> IGrainInstance.GetReminders()
        {
            return base.GetReminders();
        }

        void IGrainInstance.DeactivateOnIdle()
        {
            base.DeactivateOnIdle();
        }

        void IGrainInstance.DelayDeactivation(TimeSpan timeSpan)
        {
            base.DelayDeactivation(timeSpan);
        }

        #endregion
    }

    /// <summary>
    /// Base class for message based grains identifiable by GUID identifier
    /// </summary>
    public abstract class MessageBasedGrainWithGuidId : MessageBasedGrain, IGrainWithGuidId
    {
        /// <summary>
        /// Gets identifier of the current grain.
        /// </summary>
        /// <returns><see cref="Guid"/> identifier</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Guid Id()
        {
            return Runtime.Id(this);
        }
    }

    /// <summary>
    /// Base class for message based grains identifiable by long identifier
    /// </summary>
    public abstract class MessageBasedGrainWithLongId : MessageBasedGrain, IGrainWithLongId
    {
        /// <summary>
        /// Gets identifier of the current grain.
        /// </summary>
        /// <returns><see cref="Int64"/> identifier</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long Id()
        {
            return Runtime.Id(this);
        }
    }

    /// <summary>
    /// Base class for message based grains identifiable by string identifier
    /// </summary>
    public abstract class MessageBasedGrainWithStringId : MessageBasedGrain, IGrainWithStringId
    {
        /// <summary>
        /// Gets identifier of the current grain.
        /// </summary>
        /// <returns><see cref="String"/> identifier</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected string Id()
        {
            return Runtime.Id(this);
        }
    }

    /// <summary>
    /// Base class for persistent message based grains identifiable by GUID identifier
    /// </summary>
    public abstract class MessageBasedGrainWithGuidId<TState> : MessageBasedGrain<TState>, IGrainWithGuidId where TState : class, IGrainState
    {
        /// <summary>
        /// Gets identifier of the current grain.
        /// </summary>
        /// <returns><see cref="Guid"/> identifier</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Guid Id()
        {
            return Runtime.Id(this);
        }
    }

    /// <summary>
    /// Base class for persistent message based grains identifiable by long identifier
    /// </summary>
    public abstract class MessageBasedGrainWithLongId<TState> : MessageBasedGrain<TState>, IGrainWithLongId where TState : class, IGrainState
    {
        /// <summary>
        /// Gets identifier of the current grain.
        /// </summary>
        /// <returns><see cref="Int64"/> identifier</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected long Id()
        {
            return Runtime.Id(this);
        }
    }

    /// <summary>
    /// Base class for persistent message based grains identifiable by string identifier
    /// </summary>
    public abstract class MessageBasedGrainWithStringId<TState> : MessageBasedGrain<TState>, IGrainWithStringId where TState : class, IGrainState
    {
        /// <summary>
        /// Gets identifier of the current grain.
        /// </summary>
        /// <returns><see cref="String"/> identifier</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected string Id()
        {
            return Runtime.Id(this);
        }
    }

    /// <summary>
    /// Base class for all kinds of observable message based grains
    /// </summary>
    public abstract class ObservableMessageBasedGrain : MessageBasedGrain, IObservableGrain
    {
        public IObserverCollection Observers = new ObserverCollection();

        public Task Attach(Observes s, Type e)
        {
            Observers.Attach(s, e);
            return TaskDone.Done;
        }

        public Task Detach(Observes o, Type e)
        {
            Observers.Detach(o, e);
            return TaskDone.Done;
        }
    }

    public abstract class ObservabledMessageBasedGrainWithGuidId : ObservableMessageBasedGrain, IGrainWithGuidId
    {
        protected void Notify<TEvent>(TEvent e)
        {
            Observers.Notify(Runtime.Id(this), e);
        }
    }

    public abstract class ObservableMessageBasedGrainWithLongId : ObservableMessageBasedGrain, IGrainWithLongId
    {
        protected void Notify<TEvent>(TEvent e)
        {
            Observers.Notify(Runtime.Id(this), e);
        }
    }

    public abstract class ObservableMessageBasedGrainWithStringId : ObservableMessageBasedGrain, IGrainWithStringId
    {
        protected void Notify<TEvent>(TEvent e)
        {
            Observers.Notify(Runtime.Id(this), e);
        }
    }

    public abstract class ObservableMessageBasedGrain<TGrainState> : MessageBasedGrain<TGrainState>, IObservableGrain
        where TGrainState : class, IGrainState
    {
        public IObserverCollection Observers = new ObserverCollection();

        public Task Attach(Observes s, Type e)
        {
            Observers.Attach(s, e);
            return TaskDone.Done;
        }

        public Task Detach(Observes o, Type e)
        {
            Observers.Detach(o, e);
            return TaskDone.Done;
        }
    }

    public abstract class ObservableMessageBasedGrainWithGuidId<TGrainState> : ObservableMessageBasedGrain<TGrainState>, IGrainWithGuidId
        where TGrainState : class, IGrainState
    {
        protected void Notify<TEvent>(TEvent e)
        {
            Observers.Notify(Runtime.Id(this), e);
        }
    }

    public abstract class ObservableMessageBasedGrainWithLongId<TGrainState> : ObservableMessageBasedGrain<TGrainState>, IGrainWithLongId
        where TGrainState : class, IGrainState
    {
        protected void Notify<TEvent>(TEvent e)
        {
            Observers.Notify(Runtime.Id(this), e);
        }
    }

    public abstract class ObservableMessageBasedGrainWithStringId<TGrainState> : ObservableMessageBasedGrain<TGrainState>, IGrainWithStringId
        where TGrainState : class, IGrainState
    {
        protected void Notify<TEvent>(TEvent e)
        {
            Observers.Notify(Runtime.Id(this), e);
        }
    }

    /// <summary>
    /// This interface exists solely for unit testing purposes
    /// </summary>
    public interface IGrainInstance
    {
        IOrleansTimer RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period);
        Task<IOrleansReminder> RegisterOrUpdateReminder(string reminderName, TimeSpan dueTime, TimeSpan period);
        Task UnregisterReminder(IOrleansReminder reminder);
        Task<IOrleansReminder> GetReminder(string reminderName);
        Task<List<IOrleansReminder>> GetReminders();
        void DeactivateOnIdle();
        void DelayDeactivation(TimeSpan timeSpan);
    }
}