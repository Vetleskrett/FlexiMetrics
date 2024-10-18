<script lang="ts">
	import { page } from '$app/stores';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import ArrowUpFromLine from 'lucide-svelte/icons/arrow-up-from-line';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import DeliveriesSubmittedCard from 'src/components/DeliveriesSubmittedCard.svelte';
	import AssignmentInformationCard from 'src/components/AssignmentInformationCard.svelte';
	import DeliveryFormatCard from 'src/components/DeliveryFormatCard.svelte';
	import AnalyzersCard from 'src/components/AnalyzersCard.svelte';
	import { course, assignment, deliveryFields, teams, analyzers } from 'src/mockData';
	import CustomButton from 'src/components/CustomButton.svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;
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
			{#if !assignment.published}
				<CustomButton color="blue">
					<ArrowUpFromLine size="20" />
					<p>Publish</p>
				</CustomButton>
			{/if}
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
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8">
			<DeliveryFormatCard {deliveryFields} {assignmentId} {courseId} />
			<AnalyzersCard {analyzers} {assignmentId} {courseId} />
		</div>

		<div class="flex w-2/5 flex-col gap-8">
			<AssignmentInformationCard {assignment} />
			<DeliveriesSubmittedCard deliveriesSubmitted={32} numTeams={teams.length} />
		</div>
	</div>
</div>
