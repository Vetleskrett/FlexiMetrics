<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import ArrowUpDown from 'lucide-svelte/icons/arrow-up-down';
	import Filter from 'lucide-svelte/icons/filter';
	import List from 'lucide-svelte/icons/list';
	import { Button } from 'src/lib/components/ui/button';
	import type { AnalyzerOutput } from 'src/types';
	import { createRender, createTable, Render, Subscribe } from 'svelte-headless-table';
	import { addColumnFilters, addSortBy, addHiddenColumns } from 'svelte-headless-table/plugins';
	import { readable } from 'svelte/store';
	import StringFilter from './filters/StringFilter.svelte';
	import CustomButton from './CustomButton.svelte';
	import StringCell from './cells/StringCell.svelte';

	export let analyzerOutput: AnalyzerOutput;

	const table = createTable(readable(analyzerOutput.teamOutputs), {
		sort: addSortBy(),
		hide: addHiddenColumns(),
		filter: addColumnFilters()
	});

	const stringFilterFn = ({ filterValue, value }: { filterValue: string; value: any }) =>
		value.toString().toLowerCase().includes(filterValue.toLowerCase());

	const columns = table.createColumns([
		table.column({
			id: 'Team',
			accessor: (teamOutput) => teamOutput.teamId,
			header: 'Team',
			plugins: {
				filter: {
					fn: stringFilterFn,
					render: ({ filterValue }) => createRender(StringFilter, { filterValue })
				}
			},
			cell: ({ value }) => createRender(StringCell, { value })
		}),
		...analyzerOutput.fields.map((field, i) =>
			table.column({
				id: i.toString(),
				accessor: (teamOutput) => teamOutput.values.get(field.id),
				header: field.name,
				plugins: {
					filter: {
						fn: stringFilterFn,
						render: ({ filterValue }) => createRender(StringFilter, { filterValue })
					}
				},
				cell: ({ value }) => createRender(StringCell, { value })
			})
		)
	]);

	const { headerRows, pageRows, tableAttrs, tableBodyAttrs, pluginStates, flatColumns } =
		table.createViewModel(columns);

	const { hiddenColumnIds } = pluginStates.hide;
	let showColumnForId = Object.fromEntries(flatColumns.map((col) => [col.id, true]));
	$: $hiddenColumnIds = Object.entries(showColumnForId)
		.filter(([, show]) => !show)
		.map(([id]) => id);

	let showFilterForId = Object.fromEntries(flatColumns.map((col) => [col.id, false]));
</script>

<Card.Root class="w-[1080px] overflow-hidden">
	<Card.Header class="p-4">
		<div class="flex flex-row items-start justify-end gap-4">
			<DropdownMenu.Root>
				<DropdownMenu.Trigger asChild let:builder>
					<CustomButton color="outline" builders={[builder]}>
						<List class="h-4 w-4" />
						<p>Show/hide columns</p>
					</CustomButton>
				</DropdownMenu.Trigger>
				<DropdownMenu.Content>
					{#each analyzerOutput.fields as field, id}
						<DropdownMenu.CheckboxItem bind:checked={showColumnForId[id]}>
							{field.name}
						</DropdownMenu.CheckboxItem>
					{/each}
				</DropdownMenu.Content>
			</DropdownMenu.Root>
			<DropdownMenu.Root>
				<DropdownMenu.Trigger asChild let:builder>
					<CustomButton color="outline" builders={[builder]}>
						<Filter class="h-4 w-4" />
						<p>Filter</p>
					</CustomButton>
				</DropdownMenu.Trigger>
				<DropdownMenu.Content>
					{#each analyzerOutput.fields as field, id}
						{#if showColumnForId[id]}
							<DropdownMenu.CheckboxItem bind:checked={showFilterForId[id]}>
								{field.name}
							</DropdownMenu.CheckboxItem>
						{/if}
					{/each}
				</DropdownMenu.Content>
			</DropdownMenu.Root>
		</div>
	</Card.Header>
	<Card.Content class="p-0 ">
		<Table.Root {...$tableAttrs}>
			<Table.Header>
				{#each $headerRows as headerRow}
					<Subscribe rowAttrs={headerRow.attrs()}>
						<Table.Row>
							{#each headerRow.cells as cell (cell.id)}
								<Subscribe attrs={cell.attrs()} let:attrs props={cell.props()} let:props>
									<Table.Head {...attrs} class="px-2 font-bold text-black">
										<div class="flex h-full flex-col items-start justify-end gap-2">
											{#if props.filter?.render && showFilterForId[cell.id]}
												<Render of={props.filter.render} />
											{/if}
											<Button variant="ghost" on:click={props.sort.toggle}>
												<Render of={cell.render()} />
												<ArrowUpDown class="ml-2 h-4 w-4 " />
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
									<Table.Cell {...attrs} class="px-2">
										<div class="pl-4">
											<Render of={cell.render()} />
										</div>
									</Table.Cell>
								</Subscribe>
							{/each}
						</Table.Row>
					</Subscribe>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
