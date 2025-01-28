<script lang="ts">
	import { page } from '$app/stores';
	import TeachersCard from 'src/components/TeachersCard.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import { onMount } from 'svelte';
	import type { StudentAssignment, Teacher, StudentCourse } from 'src/types';
	import { Role } from 'src/types';
	import { getStudentCourse, getStudentAssignments } from 'src/api';
	import TeamCard from 'src/components/TeamCard.svelte';
	import { studentId } from 'src/store';
	import StudentAssignmentsCard from 'src/components/StudentAssignmentsCard.svelte';

	const courseId = $page.params.courseId;

	let course: StudentCourse;
	let assignments: StudentAssignment[] = [];
	let teachers: Teacher[] = [];

	onMount(async () => {
		try {
			course = await getStudentCourse(studentId, courseId);
			assignments = await getStudentAssignments(studentId, courseId);
			teachers = course.teachers ?? [];
		} catch (error) {
			console.error(error);
		}
	});
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/student/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{course?.code || 'loading'} - {course?.name || 'loading'}</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/knowledge-sharing.png"
				alt="knowledge-sharing"
			/>
			<div>
				<h1 class="ml-4 text-4xl font-semibold">
					{course?.code || 'loading'} - {course?.name || 'loading'}
				</h1>
				<p class="ml-4 font-semibold text-gray-500">
					{course?.year || 'loading'}
					{course?.semester || 'loading'}
				</p>
			</div>
		</div>
	</div>
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8">
			<StudentAssignmentsCard {assignments} {courseId} />
		</div>

		<div class="flex w-2/5 flex-col gap-8">
			{#if course?.team}
				<TeamCard team={course.team} />
			{/if}
			<TeachersCard userRole={Role.Student} {teachers} {courseId} />
		</div>
	</div>
</div>
