﻿namespace Api.Students.Contracts;

public class CourseStudentResponse
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
    public required int? TeamNr { get; init; }
}