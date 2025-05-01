<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb';
	import * as Table from '$lib/components/ui/table';
	import { getAnalyzerLogs } from 'src/api';
	import type {
		Analyzer,
		AnalyzerLog,
		Assignment,
		Course
	} from 'src/types/';
	import { onMount } from 'svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;
	const analyzerId = $page.params.analyzerId;

	export let data: {
		course: Course;
		assignment: Assignment;
		analyzer: Analyzer;
		analyzerLogs: AnalyzerLog[]
	};

	let logs = data.analyzerLogs;
	let interval: ReturnType<typeof setInterval>;

	onMount(() => {
		setTimeout(scrollToBottom, 100);
		interval = setInterval(updateLogs, 2000)

		return () => {
			clearInterval(interval);
		};
  	});

	const updateLogs = async () => {
		const response = await getAnalyzerLogs(analyzerId);
		if (logs.length != response.data.length) {
			logs = response.data;
			setTimeout(scrollToBottom, 100);
		}
	}

	const scrollToBottom = async () => {
    	window.scroll({ top: document.body.scrollHeight, behavior: 'smooth' });
  	};
</script>

<div class="mt-4 mb-0 flex mx-0 flex-col gap-3">
	<Breadcrumb.Root class="ml-6">
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
			<Breadcrumb.Link href="/teacher/courses/{courseId}/assignments/{assignmentId}/analyzers/{analyzerId}">
				{data.analyzer.name}
			</Breadcrumb.Link>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>Logs</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>

	<div class="w-full bg-black font-mono py-2 min-h-[calc(theme(height.dvh)-theme(space.28))]">
		<Table.Root>
			<Table.Body>
				{#each logs as log, i}
					{@const time = new Date(log.timestamp)}
					<Table.Row class="border-none p-0 m-0 h-4">
						<Table.Cell class="text-gray-500 text-xs w-12 text-right py-0 px-2 w-14 align-top">
							{i+1}
						</Table.Cell>
						<Table.Cell class="text-gray-300 text-xs w-36 py-0 px-2 align-top">
							{time.getFullYear()}-{time.getMonth()+1}-{time.getDate()}  {time.toLocaleTimeString()}
						</Table.Cell>
						<Table.Cell class="text-purple-500 text-xs py-0 px-2 align-top text-nowrap">
							[{log.category}]
						</Table.Cell>
						{#if log.type == 'Information'}
							<Table.Cell style="white-space: pre-wrap" class="text-gray-300 text-xs py-0 px-1">
								{log.text}
							</Table.Cell>
						{:else}
							<Table.Cell style="white-space: pre-wrap" class="text-red-600 text-xs py-0 px-1">
								{log.text}
							</Table.Cell>
						{/if}

					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</div>
</div>
