<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import ChevronDown from 'lucide-svelte/icons/chevron-down';
	import ChevronUp from 'lucide-svelte/icons/chevron-up';
	import ChevronsUpDown from 'lucide-svelte/icons/chevrons-up-down';
	import Filter from 'lucide-svelte/icons/filter';
	import List from 'lucide-svelte/icons/list';
	import { Button } from 'src/lib/components/ui/button';
	import { Label } from 'src/lib/components/ui/label';
	import type { Analysis, AnalysisFieldType, SlimAnalysis } from 'src/types/';
	import { createTable, Render, Subscribe } from 'svelte-headless-table';
	import { addColumnFilters, addSortBy, addHiddenColumns } from 'svelte-headless-table/plugins';
	import { readable } from 'svelte/store';
	import CustomButton from '../CustomButton.svelte';
	import { getFilter } from './filters/filters';
	import { getCell } from './cells/cells';
	import { EllipsisVertical, Trash2 } from 'lucide-svelte';
	import { ScrollArea } from 'src/lib/components/ui/scroll-area';
	import CustomAlertDialog from '../CustomAlertDialog.svelte';

	export let analyses: SlimAnalysis[];
	export let analysis: Analysis;
	export let isIndividual: boolean;
	export let onSetAnalysis: (analysisId: string) => Promise<void>;
	export let onDeleteAnalysis: () => Promise<void>;

	type Header = {
		name: string;
		type: AnalysisFieldType;
		subType?: AnalysisFieldType;
	};

	const headers: Header[] = [];

	for (let field of analysis.analysisEntries.flatMap((d) => d.fields)) {
		if (!headers.some((h) => h.name == field.name && h.type == field.type)) {
			headers.push({
				name: field.name,
				type: field.type,
				subType: field.subType
			});
		}
	}

	const table = createTable(readable(analysis.analysisEntries), {
		sort: addSortBy(),
		hide: addHiddenColumns(),
		filter: addColumnFilters()
	});

	const columns = table.createColumns([
		isIndividual
			? table.column({
					id: 'Student',
					accessor: (analysisEntry) => {
						return {
							value: analysisEntry.student?.name
						};
					},
					header: 'Student',
					plugins: {
						sort: {
							compareFn: (a: any, b: any) => a.value - b.value
						}
					},
					cell: getCell('String')
				})
			: table.column({
					id: 'Team',
					accessor: (analysisEntry) => {
						return {
							value: analysisEntry.team?.teamNr
						};
					},
					header: 'Team',
					plugins: {
						sort: {
							compareFn: (a: any, b: any) => a.value - b.value
						}
					},
					cell: getCell('String')
				}),
		...headers.map((header, i) =>
			table.column({
				id: i.toString(),
				accessor: (analysisEntry) =>
					analysisEntry.fields.find(
						(analysisField) =>
							analysisField.name == header.name && analysisField.type == header.type
					) ?? undefined,
				header: header.name,
				plugins: {
					filter: getFilter(header.type),
					sort: {
						compareFn: (a: any, b: any) => a?.value - b?.value
					}
				},
				cell: getCell(header.type, header.subType)
			})
		),
		table.column({
			id: 'Logs',
			accessor: (analysisEntry) => {
				return {
					value:
						analysisEntry.logInformation +
						(analysisEntry.logError ? '\n' + analysisEntry.logError : '')
				};
			},
			header: 'Logs',
			plugins: {
				sort: {
					compareFn: (a: any, b: any) => a.value - b.value
				}
			},
			cell: getCell('Json')
		})
	]);

	const { headerRows, pageRows, tableAttrs, tableBodyAttrs, pluginStates, flatColumns } =
		table.createViewModel(columns);

	const { hiddenColumnIds } = pluginStates.hide;
	let showColumnForId = Object.fromEntries(flatColumns.map((col) => [col.id, true]));

	showColumnForId['Logs'] = false;

	$: $hiddenColumnIds = Object.entries(showColumnForId)
		.filter(([, show]) => !show)
		.map(([id]) => id);

	let showFilterForId = Object.fromEntries(flatColumns.map((col) => [col.id, false]));

	const { sortKeys } = pluginStates.sort;
	let sortColumn: { id: string; order: 'asc' | 'desc' | undefined } | undefined = undefined;
	sortKeys.subscribe((value) => (sortColumn = value.length > 0 ? value[0] : undefined));

	sortKeys.set([
		{
			id: isIndividual ? 'Student' : 'Team',
			order: 'asc'
		}
	]);

	let showDelete = false;
</script>

<CustomAlertDialog
	bind:show={showDelete}
	description="This action cannot be undone. This will permanently delete the analysis."
	onConfirm={onDeleteAnalysis}
	action="Delete"
/>

<Card.Root class="w-[1080px]">
	<Card.Header class="px-4 py-2">
		<div class="flex items-end justify-between">
			<DropdownMenu.Root>
				<DropdownMenu.Trigger asChild let:builder>
					<Label>
						<p class="mb-[1px] ml-2 text-xs">Version</p>
						<CustomButton outline={true} color="blue" builders={[builder]}>
							<p class="text-start font-normal text-black">
								{new Date(analysis.startedAt).toDateString()}

								{new Date(analysis.startedAt).toLocaleTimeString()}
							</p>
							<ChevronDown class="h-4 w-4 text-black" />
						</CustomButton>
					</Label>
				</DropdownMenu.Trigger>
				<DropdownMenu.Content>
					{#each analyses as otherAnalysis}
						<DropdownMenu.Item on:click={() => onSetAnalysis(otherAnalysis.id)}>
							<p>
								{new Date(otherAnalysis.startedAt).toDateString()}
								{new Date(otherAnalysis.startedAt).toLocaleTimeString()}
							</p>
						</DropdownMenu.Item>
					{/each}
				</DropdownMenu.Content>
			</DropdownMenu.Root>
			{#if analysis.status == 'Completed'}
				<div class="flex items-center gap-4">
					<DropdownMenu.Root>
						<DropdownMenu.Trigger asChild let:builder>
							<CustomButton outline={true} color="blue" builders={[builder]}>
								<List class="h-4 w-4" />
								<p>Show/hide columns</p>
							</CustomButton>
						</DropdownMenu.Trigger>
						<DropdownMenu.Content>
							{#each headers as header, id}
								<DropdownMenu.CheckboxItem
									bind:checked={showColumnForId[id]}
									on:click={(e) => {
										e.preventDefault();
										showColumnForId[id] = !showColumnForId[id];
									}}
								>
									{header.name}
								</DropdownMenu.CheckboxItem>
							{/each}
							<DropdownMenu.CheckboxItem
								bind:checked={showColumnForId['Logs']}
								on:click={(e) => {
									e.preventDefault();
									showColumnForId['Logs'] = !showColumnForId['Logs'];
								}}
							>
								Logs
							</DropdownMenu.CheckboxItem>
						</DropdownMenu.Content>
					</DropdownMenu.Root>
					<DropdownMenu.Root>
						<DropdownMenu.Trigger asChild let:builder>
							<CustomButton outline={true} color="blue" builders={[builder]}>
								<Filter class="h-4 w-4" />
								<p>Filter</p>
							</CustomButton>
						</DropdownMenu.Trigger>
						<DropdownMenu.Content>
							{#each headers as header, id}
								{#if showColumnForId[id] && header.type != 'List' && header.type != 'File'}
									<DropdownMenu.CheckboxItem
										bind:checked={showFilterForId[id]}
										on:click={(e) => {
											e.preventDefault();
											showFilterForId[id] = !showFilterForId[id];
										}}
									>
										{header.name}
									</DropdownMenu.CheckboxItem>
								{/if}
							{/each}
						</DropdownMenu.Content>
					</DropdownMenu.Root>
					<DropdownMenu.Root>
						<DropdownMenu.Trigger>
							<EllipsisVertical size={24} />
						</DropdownMenu.Trigger>
						<DropdownMenu.Content>
							<DropdownMenu.Item on:click={() => (showDelete = true)}>
								<Trash2 class="h-4" />
								<p>Delete analysis</p>
							</DropdownMenu.Item>
						</DropdownMenu.Content>
					</DropdownMenu.Root>
				</div>
			{/if}
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<ScrollArea orientation="horizontal" position="both">
			<Table.Root {...$tableAttrs} style="border-collapse: collapse; border-style: hidden;">
				<Table.Header>
					{#each $headerRows as headerRow}
						<Subscribe rowAttrs={headerRow.attrs()}>
							<Table.Row>
								{#each headerRow.cells as cell (cell.id)}
									<Subscribe attrs={cell.attrs()} let:attrs props={cell.props()} let:props>
										<Table.Head {...attrs} class="p-0 font-bold text-black">
											<div class="flex h-full flex-col items-start justify-end gap-2">
												{#if props.filter?.render && showFilterForId[cell.id]}
													<div class="mt-3 px-1">
														<Render of={props.filter.render} />
													</div>
												{/if}
												<Button variant="ghost" on:click={props.sort.toggle} class="px-2">
													<Render of={cell.render()} />

													{#if sortColumn?.id == cell.id && sortColumn?.order == 'asc'}
														<ChevronUp class="ml-1 h-4 w-4" />
													{:else if sortColumn?.id == cell.id && sortColumn?.order == 'desc'}
														<ChevronDown class="ml-1 h-4 w-4" />
													{:else}
														<ChevronsUpDown class="ml-1 h-4 w-4" />
													{/if}
												</Button>
											</div>
										</Table.Head>
									</Subscribe>
								{/each}
							</Table.Row>
						</Subscribe>
					{/each}
				</Table.Header>
				<Table.Body {...$tableBodyAttrs}>
					{#each $pageRows as row (row.id)}
						<Subscribe rowAttrs={row.attrs()} let:rowAttrs>
							<Table.Row {...rowAttrs}>
								{#each row.cells as cell (cell.id)}
									<Subscribe attrs={cell.attrs()} let:attrs>
										<Table.Cell {...attrs} class="content-start border pl-4 pr-2">
											<Render of={cell.render()} />
										</Table.Cell>
									</Subscribe>
								{/each}
							</Table.Row>
						</Subscribe>
					{/each}
				</Table.Body>
			</Table.Root>
		</ScrollArea>
	</Card.Content>
</Card.Root>
