<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import FeedbacksCard from 'src/components/feedback/FeedbacksCard.svelte';
	import type { Assignment, Student, Course, Team, Feedback } from 'src/types/';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	export let data: {
		course: Course;
		assignment: Assignment;
		feedbacks: Feedback[];
		students: Student[];
		teams: Team[];
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
				<Breadcrumb.Page>Feedback</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/popular-topic--v1.png"
				alt="knowledge-sharing"
			/>
			<h1 class="ml-4 text-4xl font-semibold">Feedback</h1>
		</div>
	</div>

	<FeedbacksCard
		assignment={data.assignment}
		feedbacks={data.feedbacks}
		students={data.students}
		teams={data.teams}
	/>
</div>
