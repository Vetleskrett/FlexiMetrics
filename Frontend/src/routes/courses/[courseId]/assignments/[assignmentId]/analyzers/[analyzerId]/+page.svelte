<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';

	import { course, assignment, analyzerOutput, analyzer } from 'src/mockData';
	import AnalyzerOutputCard from 'src/components/AnalyzerOutputCard.svelte';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import Play from 'lucide-svelte/icons/play';
	import X from 'lucide-svelte/icons/x';
	import CustomButton from 'src/components/CustomButton.svelte';
	import AnalyzerRunningCard from 'src/components/AnalyzerRunningCard.svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;
	const analyzerId = $page.params.analyzerId;

	let isRunning = false;

	const onCancel = () => (isRunning = false);
	const onRun = () => (isRunning = true);
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
			<Breadcrumb.Link href="/courses/{courseId}/assignments/{assignment.id}">
				{assignment.name}
			</Breadcrumb.Link>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{analyzer.name}</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/artificial-intelligence--v2.png"
				alt="knowledge-sharing"
			/>
			<h1 class="ml-4 text-4xl font-semibold">{analyzer.name}</h1>
		</div>
		<div class="flex items-center gap-2">
			{#if isRunning}
				<CustomButton color="red" on:click={onCancel}>
					<X size="20" />
					<p>Cancel</p>
				</CustomButton>
			{:else}
				<CustomButton color="blue" on:click={onRun}>
					<Play size="20" />
					<p>Run</p>
				</CustomButton>
			{/if}
			<DropdownMenu.Root>
				<DropdownMenu.Trigger>
					<EllipsisVertical size={32} />
				</DropdownMenu.Trigger>
				<DropdownMenu.Content>
					<DropdownMenu.Item
						href="/courses/{courseId}/assignments/{assignmentId}/analyzers/{analyzerId}/edit"
					>
						<Pencil class="h-4" />
						<p>Edit analyzer</p>
					</DropdownMenu.Item>
					<DropdownMenu.Item>
						<Trash2 class="h-4" />
						<p>Delete analyzer</p>
					</DropdownMenu.Item>
				</DropdownMenu.Content>
			</DropdownMenu.Root>
		</div>
	</div>

	{#if isRunning}
		<AnalyzerRunningCard />
	{/if}

	<AnalyzerOutputCard {analyzerOutput} />
</div>
