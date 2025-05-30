﻿using Database.Models;

namespace Api.AssignmentFields.Contracts;

public class AssignmentFieldResponse
{
    public required Guid Id { get; init; }
    public required AssignmentDataType Type { get; init; }
    public required string Name { get; init; }
    public required Guid AssignmentId { get; init; }

    public required int? Min { get; init; }
    public required int? Max { get; init; }
    public required string? Regex { get; init; }
    public required AssignmentDataType? SubType { get; init; }
}