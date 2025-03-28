﻿using Database.Models;

namespace Api.AssignmentFields.Contracts;

public class UpdateAssignmentFieldsRequest
{
    public required List<AssignmentFieldRequest> Fields { get; init; }
}

public class AssignmentFieldRequest
{
    public Guid? Id { get; init; }
    public required AssignmentDataType Type { get; init; }
    public required string Name { get; init; }

    public int? Min { get; init; }
    public int? Max { get; init; }
    public string? Regex { get; init; }
    public AssignmentDataType? SubType { get; init; }
}