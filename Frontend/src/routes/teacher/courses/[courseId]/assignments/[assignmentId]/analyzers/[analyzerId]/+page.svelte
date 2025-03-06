<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import AnalysisCard from 'src/components/analyzer/AnalysisCard.svelte';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import Play from 'lucide-svelte/icons/play';
	import X from 'lucide-svelte/icons/x';
	import CustomButton from 'src/components/CustomButton.svelte';
	import AnalyzerRunningCard from 'src/components/analyzer/AnalyzerRunningCard.svelte';
	import type { Analyzer, AnalyzerAnalyses, Assignment, Course, Student, Team } from 'src/types/';
	import { ArrowDownToLine } from 'lucide-svelte';
	import { cancelAnalyzer, getAnalysis, runAnalyzer } from 'src/api';
	import * as Card from '$lib/components/ui/card/index.js';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	export let data: {
		course: Course;
		assignment: Assignment;
		analyzer: Analyzer;
		analyses: AnalyzerAnalyses;
	};

	$: analysis = data.analyses.latest;
	let isRunning = analysis?.status == 'Started' || analysis?.status == 'Running';

	const onCancel = async () => {
		isRunning = false;
		await cancelAnalyzer($page.params.analyzerId);
	};

	const onRun = async () => {
		isRunning = true;
		await runAnalyzer($page.params.analyzerId);
	};

	const onSetAnalysis = async (analysisId: string) => {
		const analysisResponse = await getAnalysis(analysisId);
		analysis = analysisResponse.data;
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
				<Breadcrumb.Page>{data.analyzer.name}</Breadcrumb.Page>
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
			<h1 class="ml-4 text-4xl font-semibold">{data.analyzer.name}</h1>
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
					<DropdownMenu.Item download href={`/api/analyzers/${$page.params.analyzerId}/script`}>
						<ArrowDownToLine class="h-4" />
						<p>Download script</p>
					</DropdownMenu.Item>
					<DropdownMenu.Item
						href="/teacher/courses/{courseId}/assignments/{assignmentId}/analyzers/{$page.params
							.analyzerId}/edit"
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

	{#key analysis}
		{#if analysis}
			<AnalysisCard
				{analysis}
				analyses={data.analyses.analyses}
				isIndividual={data.assignment.collaborationType == 'Individual'}
				{onSetAnalysis}
			/>
		{:else}
			<Card.Root class="w-[1080px]">
				<Card.Content class="text-center">
					<p>No analyses</p>
				</Card.Content>
			</Card.Root>
		{/if}
	{/key}
</div>
