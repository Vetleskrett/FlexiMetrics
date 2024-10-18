<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import ArrowUpDown from 'lucide-svelte/icons/arrow-up-down';
	import ChevronDown from 'lucide-svelte/icons/chevron-down';
	import { Button } from 'src/lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import type { AnalyzerOutput } from 'src/types';
	import { createTable, Render, Subscribe } from 'svelte-headless-table';
	import { addTableFilter, addSortBy, addHiddenColumns } from 'svelte-headless-table/plugins';
	import { readable } from 'svelte/store';

	export let analyzerOutput: AnalyzerOutput;

	const outputTypes = new Map<string, string>(
		analyzerOutput.fields.map((field) => [field.id, field.type])
	);

	const table = createTable(readable(analyzerOutput.teamOutputs), {
		sort: addSortBy(),
		filter: addTableFilter({
			fn: ({ filterValue, value }) => value.toLowerCase().includes(filterValue.toLowerCase())
		}),
		hide: addHiddenColumns()
	});

	const columns = table.createColumns([
		table.column({
			accessor: (teamOutput) => teamOutput.teamId,
			header: 'Team'
		}),
		...analyzerOutput.fields.map((field) =>
			table.column({
				accessor: (teamOutput) => teamOutput.values.get(field.id),
				header: field.name
			})
		)
	]);

	const { headerRows, pageRows, tableAttrs, tableBodyAttrs, pluginStates, flatColumns } =
		table.createViewModel(columns);

	const { filterValue } = pluginStates.filter;
	const { hiddenColumnIds } = pluginStates.hide;

	const ids = flatColumns.map((col) => col.id);
	let hideForId = Object.fromEntries(ids.map((id) => [id, true]));

	$: $hiddenColumnIds = Object.entries(hideForId)
		.filter(([, hide]) => !hide)
		.map(([id]) => id);

	const hidableColumns = analyzerOutput.fields.map((field) => field.name);
</script>

<Card.Root class="w-[1080px] overflow-hidden p-0">
	<Card.Header class="flex flex-row items-center justify-end gap-8 py-4">
		<Input class="max-w-sm" placeholder="Filter" type="text" bind:value={$filterValue} />
		<DropdownMenu.Root>
			<DropdownMenu.Trigger asChild let:builder>
				<Button variant="outline" builders={[builder]}>
					Show/hide columns <ChevronDown class="ml-2 h-4 w-4" />
				</Button>
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				{#each flatColumns as col}
					{#if hidableColumns.includes(col.id)}
						<DropdownMenu.CheckboxItem bind:checked={hideForId[col.id]}>
							{col.header}
						</DropdownMenu.CheckboxItem>
					{/if}
				{/each}
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</Card.Header>
	<Card.Content class="p-0">
		<Table.Root {...$tableAttrs}>
			<Table.Header>
				{#each $headerRows as headerRow}
					<Subscribe rowAttrs={headerRow.attrs()}>
						<Table.Row>
							{#each headerRow.cells as cell (cell.id)}
								<Subscribe attrs={cell.attrs()} let:attrs props={cell.props()} let:props>
									<Table.Head {...attrs} class="px-2 font-bold text-black">
										<Button variant="ghost" on:click={props.sort.toggle}>
											<Render of={cell.render()} />
											<ArrowUpDown class={'ml-2 h-4 w-4'} />
										</Button>
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
