<script lang="ts">
	import { page } from '$app/stores';
	import TeachersCard from 'src/components/teacher/TeachersCard.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import type { StudentAssignment, Course, Team, Teacher } from 'src/types/';
	import { Role } from 'src/types/';
	import TeamCard from 'src/components/team/TeamCard.svelte';
	import StudentAssignmentsCard from 'src/components/assignment/StudentAssignmentsCard.svelte';
	import TeamsCard from 'src/components/team/TeamsCard.svelte';

	const courseId = $page.params.courseId;

	export let data: {
		course: Course;
		assignments: StudentAssignment[];
		team: Team | null;
		teams: Team[] | null;
		teachers: Teacher[];
	};
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/student/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{data.course.code} - {data.course.name}</Breadcrumb.Page>
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
					{data.course.code} - {data.course.name}
				</h1>
				<p class="ml-4 font-semibold text-gray-500">
					{data.course.year}
					{data.course.semester}
				</p>
			</div>
		</div>
	</div>
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8">
			<StudentAssignmentsCard assignments={data.assignments} {courseId} />
		</div>

		<div class="flex w-2/5 flex-col gap-8">
			{#if data.team}
				<TeamCard team={data.team} />
			{:else if data.teams}
				<TeamsCard {courseId} numTeams={data.teams.length} userRole={Role.Student} />
			{/if}
			<TeachersCard userRole={Role.Student} teachers={data.teachers} />
		</div>
	</div>
</div>
