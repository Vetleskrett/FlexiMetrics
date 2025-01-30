<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import MoveRight from 'lucide-svelte/icons/move-right';
	import AssignmentInformationCard from 'src/components/AssignmentInformationCard.svelte';
	import { onMount } from 'svelte';
	import {
		getCourse,
		getAssignment,
		getAssignmentFields,
		getStudentDelivery,
		getStudentFeedback
	} from 'src/api';
	import {
		Role,
		type Assignment,
		type AssignmentField,
		type Course,
		type Delivery,
		type Feedback
	} from 'src/types';
	import AssignmentDescriptionCard from 'src/components/AssignmentDescriptionCard.svelte';
	import DeliveryCard from 'src/components/DeliveryCard.svelte';
	import { studentId } from 'src/store';
	import FeedbackCard from 'src/components/FeedbackCard.svelte';
	import CustomButton from 'src/components/CustomButton.svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	let course: Course;
	let assignment: Assignment;
	let assignmentFields: AssignmentField[];
	let delivery: Delivery | null;
	let feedback: Feedback | null;

	onMount(async () => {
		try {
			course = await getCourse(courseId);
			assignment = await getAssignment(assignmentId);
			assignmentFields = await getAssignmentFields(assignmentId);
			try {
				delivery = await getStudentDelivery(studentId, assignmentId);
				feedback = await getStudentFeedback(studentId, assignmentId);
			} catch (error) {}
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
				<Breadcrumb.Link href="/student/courses/{courseId}"
					>{course?.code} - {course?.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{assignment?.name}</Breadcrumb.Page>
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
			<h1 class="ml-4 text-4xl font-semibold">{assignment?.name}</h1>
		</div>
		<CustomButton
			color="green"
			href="/student/courses/{assignment?.courseId}/assignments/{assignment?.id}/delivery"
		>
			<MoveRight />
			<p>New Delivery</p>
		</CustomButton>
	</div>
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8">
			<AssignmentDescriptionCard {assignment} />
			<DeliveryCard {assignmentFields} {delivery} />
		</div>
		<div class="flex w-2/5 flex-col gap-8">
			<AssignmentInformationCard userRole={Role.Student} {assignment} />
			<FeedbackCard {feedback} {assignment} />
		</div>
	</div>
</div>
