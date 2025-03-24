<script lang="ts">
	import { page } from '$app/stores';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import ArrowUpFromLine from 'lucide-svelte/icons/arrow-up-from-line';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb';
	import CompletedTotalCard from 'src/components/CompletedTotalCard.svelte';
	import AssignmentInformationCard from 'src/components/assignment/AssignmentInformationCard.svelte';
	import AssignmentFormatCard from 'src/components/assignment/AssignmentFormatCard.svelte';
	import AnalyzersCard from 'src/components/analyzer/AnalyzersCard.svelte';
	import CustomButton from 'src/components/CustomButton.svelte';
	import type {
		Assignment,
		Course,
		AssignmentField,
		Delivery,
		Student,
		Team,
		Analyzer
	} from 'src/types/';
	import { Role } from 'src/types/';
	import { goto } from '$app/navigation';
	import { deleteAssigment, putAssignment } from 'src/api';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	export let data: {
		course: Course;
		assignment: Assignment;
		assignmentFields: AssignmentField[];
		deliveries: Delivery[];
		students: Student[];
		teams: Team[];
		analyzers: Analyzer[];
	};

	async function publishAssignmentButton() {
		try {
			const response = await putAssignment(assignmentId, {
				...data.assignment,
				published: true
			});
			data.assignment = response.data;
		} catch (exception) {
			console.error('Something Went Wrong!');
		}
	}

	async function deleteAssignmentButton() {
		try {
			await deleteAssigment(assignmentId);
			goto(`/teacher/courses/${courseId}`);
		} catch (exception) {
			console.error('Something Went Wrong!');
		}
	}
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
			{#if !data.assignment.published}
				<p class="ml-4 text-2xl font-semibold text-red-500">DRAFT</p>
			{/if}
		</div>

		<div class="flex items-center gap-2">
			{#if !data.assignment.published}
				<CustomButton color="blue" on:click={publishAssignmentButton}>
					<ArrowUpFromLine size="20" />
					<p>Publish</p>
				</CustomButton>
			{/if}
			<DropdownMenu.Root>
				<DropdownMenu.Trigger>
					<EllipsisVertical size={32} />
				</DropdownMenu.Trigger>
				<DropdownMenu.Content>
					<DropdownMenu.Item href="{assignmentId}/edit">
						<Pencil class="h-4" />
						<p>Edit assignment</p>
					</DropdownMenu.Item>
					<DropdownMenu.Item on:click={deleteAssignmentButton}>
						<Trash2 class="h-4" />
						<p>Delete assignment</p>
					</DropdownMenu.Item>
				</DropdownMenu.Content>
			</DropdownMenu.Root>
		</div>
	</div>
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8">
			<AssignmentFormatCard assignmentFields={data.assignmentFields} {assignmentId} {courseId} />
			<AnalyzersCard analyzers={data.analyzers} {assignmentId} {courseId} />
		</div>

		<div class="flex w-2/5 flex-col gap-8">
			<AssignmentInformationCard userRole={Role.Teacher} assignment={data.assignment} />
			<CompletedTotalCard
				completed={data.deliveries.length}
				total={data.assignment.collaborationType == 'Individual'
					? data.students.length
					: data.teams.length}
				headline={'Deliveries Submitted'}
			/>
		</div>
	</div>
</div>
