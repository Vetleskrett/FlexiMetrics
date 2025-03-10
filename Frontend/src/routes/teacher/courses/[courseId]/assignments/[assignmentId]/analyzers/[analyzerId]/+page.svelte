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
	import type { Analysis, Analyzer, AnalyzerAnalyses, Assignment, Course } from 'src/types/';
	import { ArrowDownToLine } from 'lucide-svelte';
	import {
		cancelAnalyzer,
		deleteAnalysis,
		getAnalysis,
		getAnalyzerStatusEventSource,
		getAnalyzerAnalyses,
		runAnalyzer
	} from 'src/api';
	import * as Card from '$lib/components/ui/card/index.js';
	import { onDestroy, onMount } from 'svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	export let data: {
		course: Course;
		assignment: Assignment;
		analyzer: Analyzer;
		analyses: AnalyzerAnalyses;
	};

	let analysis = data.analyses.latest;

	let status = {
		total: 30,
		completed: analysis?.analysisEntries?.length || 0,
		logs: ''
	};

	const update = async () => {
		closeEventSource();
		const response = await getAnalyzerAnalyses($page.params.analyzerId);
		data.analyses = response.data;
		analysis = data.analyses.latest;
		subscribeToStatusIfRunning();
	};

	type StatusUpdate = {
		analysis: Analysis;
		logs: string;
	};

	let eventSource: EventSource | undefined = undefined;

	const closeEventSource = () => {
		if (eventSource) {
			console.log('Closing event source');
			eventSource?.close();
			eventSource = undefined;
		}
	};

	const subscribeToStatus = () => {
		closeEventSource();

		console.log('Subscribing to event source');

		eventSource = getAnalyzerStatusEventSource($page.params.analyzerId);

		eventSource.onmessage = async (event) => {
			const statusUpdate = JSON.parse(event.data) as StatusUpdate;
			console.log('Received event:', statusUpdate);

			analysis = statusUpdate.analysis;

			status.completed = analysis?.analysisEntries.length || 0;
			status.logs += statusUpdate.logs + '\n';
		};

		eventSource.onerror = async (err) => {
			console.error('EventSource failed:', err);
			closeEventSource();
		};
	};

	const subscribeToStatusIfRunning = () => {
		if (analysis?.status == 'Started' || analysis?.status == 'Running') {
			subscribeToStatus();
		}
	};

	onMount(() => {
		subscribeToStatusIfRunning();
	});

	onDestroy(() => {
		closeEventSource();
	});

	const onCancel = async () => {
		await cancelAnalyzer($page.params.analyzerId);
		await update();
	};

	const onRun = async () => {
		await runAnalyzer($page.params.analyzerId);
		await update();
	};

	const onSetAnalysis = async (analysisId: string) => {
		closeEventSource();
		const analysisResponse = await getAnalysis(analysisId);
		analysis = analysisResponse.data;
		subscribeToStatusIfRunning();
	};

	const onDeleteAnalysis = async () => {
		if (analysis) {
			await deleteAnalysis(analysis.id);
			await update();
		}
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
			{#if analysis?.status == 'Started' || analysis?.status == 'Running'}
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

	{#if analysis}
		{#if analysis?.status == 'Started' || analysis?.status == 'Running'}
			<AnalyzerRunningCard {status} />
		{/if}

		<AnalysisCard
			{analysis}
			analyses={data.analyses.analyses}
			isIndividual={data.assignment.collaborationType == 'Individual'}
			{onSetAnalysis}
			{onDeleteAnalysis}
		/>
	{:else}
		<Card.Root class="w-[1080px]">
			<Card.Content class="text-center">
				<p>No analyses</p>
			</Card.Content>
		</Card.Root>
	{/if}
</div>
