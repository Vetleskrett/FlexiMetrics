<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import DeliveryCard from 'src/components/DeliveryCard.svelte';
	import FeedbackFormCard from 'src/components/FeedbackFormCard.svelte';
	import type { Assignment, Course, Delivery, Feedback, AssignmentField, Team } from 'src/types';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;
	const teamId = $page.params.teamId;

	export let data: {
		course: Course;
		assignment: Assignment;
		assignmentFields: AssignmentField[];
		delivery: Delivery | null;
		feedback: Feedback | null;
		team: Team;
	};
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses/{courseId}"
					>{data.course.code} - {data.course.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Link href="/teacher/courses/{courseId}/assignments/{assignmentId}">
				{data.assignment.name}
			</Breadcrumb.Link>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>Team {data.team.teamNr}</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/group.png"
				alt="knowledge-sharing"
			/>
			<h1 class="ml-4 text-4xl font-semibold">Team {data.team.teamNr}</h1>
		</div>
	</div>
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8">
			<FeedbackFormCard assignment={data.assignment} feedback={data.feedback} {teamId} />
		</div>

		<div class="flex w-2/5 flex-col gap-8">
			<DeliveryCard assignmentFields={data.assignmentFields} delivery={data.delivery} />
		</div>
	</div>
</div>
