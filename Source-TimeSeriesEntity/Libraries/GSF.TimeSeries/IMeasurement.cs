﻿//******************************************************************************************************
//  IMeasurement.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/02/2010 - J. Ritchie Carroll
//       Generated original version of source code.
//  04/14/2011 - J. Ritchie Carroll
//       Added received and published timestamps for measurements.
//  10/16/2013 - Stephen C. Wills
//       Redefined the IMeasurement interface to IMeasurement<T> to allow for measurements with values
//       of different types.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace GSF.TimeSeries
{
    /// <summary>
    /// Method signature for function used to apply a downsampling filter over a sequence of <see cref="IMeasurement{T}"/> values.
    /// </summary>
    /// <param name="source">Sequence of <see cref="IMeasurement{T}"/> values over which to apply filter.</param>
    /// <returns>Result of filter applied to sequence of <see cref="IMeasurement{T}"/> values.</returns>
    public delegate T TimeSeriesFilterFunction<T>(IEnumerable<T> source) where T : ITimeSeriesEntity;

    #region [ Enumerations ]

    /// <summary>
    /// Measurement state flags.
    /// </summary>
    [Flags]
    public enum MeasurementStateFlags : uint
    {
        /// <summary>
        /// Defines normal state.
        /// </summary>
        Normal = (uint)Bits.Nil,
        /// <summary>
        /// Defines bad data state.
        /// </summary>
        BadData = (uint)Bits.Bit00,
        /// <summary>
        /// Defines suspect data state.
        /// </summary>
        SuspectData = (uint)Bits.Bit01,
        /// <summary>
        /// Defines over range error, i.e., unreasonable high value.
        /// </summary>
        OverRangeError = (uint)Bits.Bit02,
        /// <summary>
        /// Defines under range error, i.e., unreasonable low value.
        /// </summary>
        UnderRangeError = (uint)Bits.Bit03,
        /// <summary>
        /// Defines alarm for high value.
        /// </summary>
        AlarmHigh = (uint)Bits.Bit04,
        /// <summary>
        /// Defines alarm for low value.
        /// </summary>
        AlarmLow = (uint)Bits.Bit05,
        /// <summary>
        /// Defines warning for high value.
        /// </summary>
        WarningHigh = (uint)Bits.Bit06,
        /// <summary>
        /// Defines warning for low value.
        /// </summary>
        WarningLow = (uint)Bits.Bit07,
        /// <summary>
        /// Defines alarm for flat-lined value, i.e., latched value test alarm.
        /// </summary>
        FlatlineAlarm = (uint)Bits.Bit08,
        /// <summary>
        /// Defines comparison alarm, i.e., outside threshold of comparison with a real-time value.
        /// </summary>
        ComparisonAlarm = (uint)Bits.Bit09,
        /// <summary>
        /// Defines rate-of-change alarm.
        /// </summary>
        ROCAlarm = (uint)Bits.Bit10,
        /// <summary>
        /// Defines bad value received.
        /// </summary>
        ReceivedAsBad = (uint)Bits.Bit11,
        /// <summary>
        /// Defines calculated value state.
        /// </summary>
        CalcuatedValue = (uint)Bits.Bit12,
        /// <summary>
        /// Defines calculation error with the value.
        /// </summary>
        CalculationError = (uint)Bits.Bit13,
        /// <summary>
        /// Defines calculation warning with the value.
        /// </summary>
        CalculationWarning = (uint)Bits.Bit14,
        /// <summary>
        /// Defines reserved quality flag.
        /// </summary>
        ReservedQualityFlag = (uint)Bits.Bit15,
        /// <summary>
        /// Defines bad time state.
        /// </summary>
        BadTime = (uint)Bits.Bit16,
        /// <summary>
        /// Defines suspect time state.
        /// </summary>
        SuspectTime = (uint)Bits.Bit17,
        /// <summary>
        /// Defines late time alarm.
        /// </summary>
        LateTimeAlarm = (uint)Bits.Bit18,
        /// <summary>
        /// Defines future time alarm.
        /// </summary>
        FutureTimeAlarm = (uint)Bits.Bit19,
        /// <summary>
        /// Defines up-sampled state.
        /// </summary>
        UpSampled = (uint)Bits.Bit20,
        /// <summary>
        /// Defines down-sampled state.
        /// </summary>
        DownSampled = (uint)Bits.Bit21,
        /// <summary>
        /// Defines discarded value state.
        /// </summary>
        DiscardedValue = (uint)Bits.Bit22,
        /// <summary>
        /// Defines reserved time flag.
        /// </summary>
        ReservedTimeFlag = (uint)Bits.Bit23,
        /// <summary>
        /// Defines user defined flag 1.
        /// </summary>
        UserDefinedFlag1 = (uint)Bits.Bit24,
        /// <summary>
        /// Defines user defined flag 2.
        /// </summary>
        UserDefinedFlag2 = (uint)Bits.Bit25,
        /// <summary>
        /// Defines user defined flag 3.
        /// </summary>
        UserDefinedFlag3 = (uint)Bits.Bit26,
        /// <summary>
        /// Defines user defined flag 4.
        /// </summary>
        UserDefinedFlag4 = (uint)Bits.Bit27,
        /// <summary>
        /// Defines user defined flag 5.
        /// </summary>
        UserDefinedFlag5 = (uint)Bits.Bit28,
        /// <summary>
        /// Defines system error state.
        /// </summary>
        SystemError = (uint)Bits.Bit29,
        /// <summary>
        /// Defines system warning state.
        /// </summary>
        SystemWarning = (uint)Bits.Bit30,
        /// <summary>
        /// Defines measurement error flag.
        /// </summary>
        MeasurementError = (uint)Bits.Bit31
    }

    #endregion

    /// <summary>
    /// Represents an interface for an abstract measurement value measured by a device at an exact time.
    /// </summary>
    /// <remarks>
    /// This interface abstractly represents a measured value at an exact time interval.
    /// </remarks>
    public interface IMeasurement<out T> : ITimeSeriesEntity
    {
        /// <summary>
        /// Gets <see cref="MeasurementStateFlags"/> associated with this <see cref="IMeasurement{T}"/>.
        /// </summary>
        MeasurementStateFlags StateFlags
        {
            get;
        }

        /// <summary>
        /// Gets the raw value of this <see cref="IMeasurement{T}"/>.
        /// </summary>
        T Value
        {
            get;
        }
    }

    /// <summary>
    /// Defines static extension functions for <see cref="IMeasurement{T}"/> implementations.
    /// </summary>
    /// <remarks>
    /// These helper functions map to the previously defined corresponding properties to help with the transition of <see cref="MeasurementStateFlags"/>.
    /// </remarks>
    public static class MeasurementExtensions
    {
        /// <summary>
        /// Returns <c>true</c> if <see cref="MeasurementStateFlags.BadData"/> is not set.
        /// </summary>
        /// <param name="measurement"><see cref="IMeasurement{T}"/> instance to test.</param>
        /// <returns><c>true</c> if <see cref="MeasurementStateFlags.BadData"/> is not set.</returns>
        public static bool ValueQualityIsGood<T>(this IMeasurement<T> measurement)
        {
            return (measurement.StateFlags & MeasurementStateFlags.BadData) == 0;
        }

        /// <summary>
        /// Returns <c>true</c> if <see cref="MeasurementStateFlags.BadTime"/> is not set.
        /// </summary>
        /// <param name="measurement"><see cref="IMeasurement{T}"/> instance to test.</param>
        /// <returns><c>true</c> if <see cref="MeasurementStateFlags.BadTime"/> is not set.</returns>
        public static bool TimestampQualityIsGood<T>(this IMeasurement<T> measurement)
        {
            return (measurement.StateFlags & MeasurementStateFlags.BadTime) == 0;
        }

        /// <summary>
        /// Returns <c>true</c> if <see cref="MeasurementStateFlags.DiscardedValue"/> is set.
        /// </summary>
        /// <param name="measurement"><see cref="IMeasurement{T}"/> instance to test.</param>
        /// <returns><c>true</c> if <see cref="MeasurementStateFlags.DiscardedValue"/> is not set.</returns>
        public static bool IsDiscarded<T>(this IMeasurement<T> measurement)
        {
            return (measurement.StateFlags & MeasurementStateFlags.DiscardedValue) > 0;
        }

        /// <summary>
        /// Returns <c>true</c> if <see cref="MeasurementStateFlags.CalcuatedValue"/> is set.
        /// </summary>
        /// <param name="measurement"><see cref="IMeasurement{T}"/> instance to test.</param>
        /// <returns><c>true</c> if <see cref="MeasurementStateFlags.CalcuatedValue"/> is not set.</returns>
        public static bool IsCalculated<T>(this IMeasurement<T> measurement)
        {
            return (measurement.StateFlags & MeasurementStateFlags.CalcuatedValue) > 0;
        }
    }
}