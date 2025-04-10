<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import AnalysisCard from 'src/components/analyzer/AnalysisCard.svelte';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import Play from 'lucide-svelte/icons/play';
	import X from 'lucide-svelte/icons/x';
	import CustomButton from 'src/components/CustomButton.svelte';
	import type {
		Analysis,
		Analyzer,
		AnalyzerAnalyses,
		AnalyzerStatusUpdate,
		Assignment,
		Course
	} from 'src/types/';
	import { ArrowDownToLine } from 'lucide-svelte';
	import {
		deleteAnalyzer,
		cancelAnalyzer,
		deleteAnalysis,
		getAnalysis,
		getAnalyzerStatusEventSource,
		getAnalyzerAnalyses,
		runAnalyzer
	} from 'src/api';
	import * as Card from '$lib/components/ui/card';
	import { onDestroy, afterUpdate } from 'svelte';
	import { goto } from '$app/navigation';
	import { handleErrors } from 'src/utils';
	import CustomAlertDialog from 'src/components/CustomAlertDialog.svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	export let data: {
		course: Course;
		assignment: Assignment;
		analyzer: Analyzer;
		analyses: AnalyzerAnalyses;
	};

	$: analysis = data.analyses.latest;

	type RunningAnalyzerInfo = {
		eventSource: EventSource;
		analyzerId: string;
		analysis: Analysis;
	};

	let runningAnalyzerInfo: RunningAnalyzerInfo | undefined = undefined;

	const subscribeToEventSource = () => {
		if (!analysis) {
			return;
		}
		if (analysis.status != 'Running') {
			return;
		}
		if (runningAnalyzerInfo) {
			return;
		}

		console.log('Subscribing to event source');

		runningAnalyzerInfo = {
			eventSource: getAnalyzerStatusEventSource($page.params.analyzerId),
			analyzerId: $page.params.analyzerId,
			analysis: analysis
		};

		runningAnalyzerInfo.eventSource.onmessage = async (event) => {
			const statusUpdate = JSON.parse(event.data) as AnalyzerStatusUpdate;

			data.analyzer = statusUpdate.analyzer;

			if (statusUpdate.analysis == null) {
				update();
				return;
			}

			runningAnalyzerInfo!.analysis = statusUpdate.analysis;

			if (runningAnalyzerInfo!.analysis.id == analysis?.id) {
				analysis = runningAnalyzerInfo!.analysis;
			}
		};

		runningAnalyzerInfo.eventSource.onerror = async () => {
			closeEventSource();
		};
	};

	const closeEventSource = () => {
		if (runningAnalyzerInfo) {
			console.log('Closing event source');
			runningAnalyzerInfo.eventSource.close();
			runningAnalyzerInfo = undefined;
		}
	};

	const update = async () => {
		closeEventSource();

		await handleErrors(async () => {
			const response = await getAnalyzerAnalyses($page.params.analyzerId);
			data.analyses = response.data;
			analysis = data.analyses.latest;
		});

		subscribeToEventSource();
	};

	onDestroy(() => {
		closeEventSource();
	});

	afterUpdate(() => {
		if (runningAnalyzerInfo?.analyzerId != $page.params.analyzerId) {
			closeEventSource();
		}

		subscribeToEventSource();
	});

	const onCancel = async () => {
		await handleErrors(async () => {
			await cancelAnalyzer($page.params.analyzerId);
		});
	};

	const onRun = async () => {
		await handleErrors(async () => {
			await runAnalyzer($page.params.analyzerId);
			await update();
		});
	};

	const onSetAnalysis = async (analysisId: string) => {
		if (analysisId == runningAnalyzerInfo?.analysis.id) {
			analysis = runningAnalyzerInfo.analysis;
		} else {
			await handleErrors(async () => {
				const analysisResponse = await getAnalysis(analysisId);
				analysis = analysisResponse.data;
			});
		}
	};

	const onDeleteAnalysis = async () => {
		await handleErrors(async () => {
			await deleteAnalysis(analysis!.id);
			await update();
		});
	};

	let showDelete = false;
	const onDeleteAnalyzer = async () => {
		await handleErrors(async () => {
			await deleteAnalyzer($page.params.analyzerId);
			goto(`/teacher/courses/${courseId}/assignments/${assignmentId}`, {
				invalidateAll: true
			});
		});
	};
</script>

<CustomAlertDialog
	bind:show={showDelete}
	description="This action cannot be undone. This will permanently delete the analyzer."
	onConfirm={onDeleteAnalyzer}
	action="Delete"
/>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses/{courseId}">
					{data.course.code} - {data.course.name}
				</Breadcrumb.Link>
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
			{#if data.analyzer.state == 'Building'}
				<p>building...</p>
			{:else if data.analyzer.state == 'Running'}
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
					<DropdownMenu.Item on:click={() => (showDelete = true)}>
						<Trash2 class="h-4" />
						<p>Delete analyzer</p>
					</DropdownMenu.Item>
				</DropdownMenu.Content>
			</DropdownMenu.Root>
		</div>
	</div>

	{#if analysis}
		{#key analysis}
			<AnalysisCard
				{analysis}
				analyses={data.analyses.analyses}
				isIndividual={data.assignment.collaborationType == 'Individual'}
				{onSetAnalysis}
				{onDeleteAnalysis}
			/>
		{/key}
	{:else}
		<Card.Root class="w-[1080px]">
			<Card.Content class="text-center">
				<p>No analyses</p>
			</Card.Content>
		</Card.Root>
	{/if}
</div>
