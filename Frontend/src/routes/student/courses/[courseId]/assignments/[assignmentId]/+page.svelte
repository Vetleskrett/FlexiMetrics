<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import MoveRight from 'lucide-svelte/icons/move-right';
	import AssignmentInformationCard from 'src/components/assignment/AssignmentInformationCard.svelte';
	import {
		Role,
		type Assignment,
		type AssignmentField,
		type Course,
		type Delivery,
		type Feedback
	} from 'src/types/';
	import AssignmentDescriptionCard from 'src/components/assignment/AssignmentDescriptionCard.svelte';
	import DeliveryCard from 'src/components/delivery/DeliveryCard.svelte';
	import FeedbackCard from 'src/components/feedback/FeedbackCard.svelte';
	import CustomButton from 'src/components/CustomButton.svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	export let data: {
		course: Course;
		assignment: Assignment;
		assignmentFields: AssignmentField[];
		delivery: Delivery | null;
		feedback: Feedback | null;
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
				<Breadcrumb.Link href="/student/courses/{courseId}"
					>{data.course.code} - {data.course.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{data.assignment.name}</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/edit-text-file.png"
				alt="knowledge-sharing"
			/>
			<h1 class="ml-4 text-4xl font-semibold">{data.assignment.name}</h1>
		</div>
		{#if new Date(data.assignment.dueDate) > new Date()}
			<CustomButton
				color="green"
				href="/student/courses/{courseId}/assignments/{assignmentId}/delivery"
			>
				<MoveRight />
				<p>New Delivery</p>
			</CustomButton>
		{:else}
			<p class="font-semibold text-gray-500">Due date expired</p>
		{/if}
	</div>
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8">
			<AssignmentDescriptionCard assignment={data.assignment} />
			<DeliveryCard assignmentFields={data.assignmentFields} delivery={data.delivery} />
		</div>
		<div class="flex w-2/5 flex-col gap-8">
			<AssignmentInformationCard userRole={Role.Student} assignment={data.assignment} />
			<FeedbackCard feedback={data.feedback} assignment={data.assignment} />
		</div>
	</div>
</div>
