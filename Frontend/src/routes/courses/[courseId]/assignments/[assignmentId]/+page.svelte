<script lang="ts">
	import { page } from '$app/stores';
	import type { Analyzer, DeliveryField, Student, Team } from 'src/types';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import ArrowUpFromLine from 'lucide-svelte/icons/arrow-up-from-line';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import Button from '$lib/components/ui/button/button.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import DeliveriesSubmittedCard from 'src/components/DeliveriesSubmittedCard.svelte';
	import AssignmentInformationCard from 'src/components/AssignmentInformationCard.svelte';
	import DeliveryFormatCard from 'src/components/DeliveryFormatCard.svelte';
	import AnalyzersCard from 'src/components/AnalyzersCard.svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	const course = {
		id: '1',
		code: 'TDT101',
		name: 'Programmering',
		year: 2024,
		semester: 'Autumn'
	};

	const assignment = {
		id: '1',
		name: 'Assignment 1',
		due: '18.09.2024',
		individual: false,
		published: false
	};
	const students: Student[] = [];

	for (let i = 1; i <= 196; i++) {
		students.push({
			id: i.toString(),
			email: 'ola@ntnu.no'
		});
	}

	const teams: Team[] = [];
	for (let i = 1; i <= 49; i++) {
		teams.push({
			id: i.toString(),
			students: []
		});
	}

	const deliveryFields: DeliveryField[] = [
		{
			id: '1',
			name: 'Team Name',
			type: 'String'
		},
		{
			id: '2',
			name: 'Source Code',
			type: 'File'
		},
		{
			id: '3',
			name: 'Url',
			type: 'String'
		}
	];

	const analyzers: Analyzer[] = [
		{
			id: '1',
			name: 'Git Analyzer'
		},
		{
			id: '2',
			name: 'Lighthouse Analyzer'
		},
		{
			id: '3',
			name: 'Code Analyzer'
		}
	];
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/courses/{courseId}">{course.code} - {course.name}</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{assignment.name}</Breadcrumb.Page>
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
			<h1 class="ml-4 text-4xl font-semibold">{assignment.name}</h1>
			{#if !assignment.published}
				<p class="ml-4 text-2xl font-semibold text-gray-500">DRAFT</p>
			{/if}
		</div>

		<div class="flex items-center gap-2">
			<Button
				class="flex h-9 flex-row justify-between gap-2 bg-button-blue pl-3 pr-3 hover:bg-button-blue-hover"
			>
				<ArrowUpFromLine size="20" />
				<p>Publish</p>
			</Button>
			<DropdownMenu.Root>
				<DropdownMenu.Trigger>
					<EllipsisVertical size={32} />
				</DropdownMenu.Trigger>
				<DropdownMenu.Content>
					<DropdownMenu.Item href="/courses/{courseId}/assignments/{assignmentId}/edit">
						<Pencil class="h-4" />
						<p>Edit assignment</p>
					</DropdownMenu.Item>
					<DropdownMenu.Item>
						<Trash2 class="h-4" />
						<p>Delete assignment</p>
					</DropdownMenu.Item>
				</DropdownMenu.Content>
			</DropdownMenu.Root>
		</div>
	</div>
	<div class="flex flex-row gap-8">
		<div class="flex w-[700px] flex-col gap-8">
			<DeliveryFormatCard {deliveryFields} {assignmentId} {courseId} />
			<AnalyzersCard {analyzers} {assignmentId} {courseId} />
		</div>

		<div class="flex w-[400px] flex-col gap-8">
			<AssignmentInformationCard {assignment} />
			<DeliveriesSubmittedCard deliveriesSubmitted={32} numTeams={teams.length} />
		</div>
	</div>
</div>
