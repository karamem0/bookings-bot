//
// Copyright (c) 2021-2025 karamem0
//
// This software is released under the MIT License.
//
// https://github.com/karamem0/bookings-bot/blob/main/LICENSE
//

using Karamem0.BookingsBot.Steps.Abstraction;

namespace Karamem0.BookingsBot.Steps;

public class BookingStepCollection(params Step[] collection) : StepCollection(collection);
