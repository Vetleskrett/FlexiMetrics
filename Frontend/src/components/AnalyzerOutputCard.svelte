<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import ChevronDown from 'lucide-svelte/icons/chevron-down';
	import ChevronUp from 'lucide-svelte/icons/chevron-up';
	import ChevronsUpDown from 'lucide-svelte/icons/chevrons-up-down';
	import Filter from 'lucide-svelte/icons/filter';
	import List from 'lucide-svelte/icons/list';
	import { Button } from 'src/lib/components/ui/button';
	import { Label } from 'src/lib/components/ui/label';
	import type { AnalyzerOutput, AnalyzerOutputVersion } from 'src/types';
	import { createTable, Render, Subscribe } from 'svelte-headless-table';
	import { addColumnFilters, addSortBy, addHiddenColumns } from 'svelte-headless-table/plugins';
	import { readable } from 'svelte/store';
	import CustomButton from './CustomButton.svelte';
	import { getFilter } from './filters/filters';
	import { getCell } from './cells/cells';

	export let analyzerOutput: AnalyzerOutput;

	const table = createTable(readable(analyzerOutput.teamOutputs), {
		sort: addSortBy(),
		hide: addHiddenColumns(),
		filter: addColumnFilters()
	});

	const columns = table.createColumns([
		table.column({
			id: 'Team',
			accessor: (teamOutput) => teamOutput.teamId,
			header: 'Team',
			cell: getCell({ id: 'Team', name: 'Team', type: 'String' })
		}),
		...analyzerOutput.fields.map((field, i) =>
			table.column({
				id: i.toString(),
				accessor: (teamOutput) => teamOutput.values.get(field.id),
				header: field.name,
				plugins: {
					filter: getFilter(field.type)
				},
				cell: getCell(field)
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

	const { sortKeys } = pluginStates.sort;
	let sortColumn: { id: string; order: 'asc' | 'desc' | undefined } | undefined = undefined;
	sortKeys.subscribe((value) => (sortColumn = value.length > 0 ? value[0] : undefined));

	let currentVersion = analyzerOutput.currentVersion;
	const onVersionChange = (version: AnalyzerOutputVersion) => {
		currentVersion = version;
	};
</script>

<Card.Root class="w-[1080px]">
	<Card.Header class="px-4 py-2">
		<div class="flex items-end justify-between">
			<DropdownMenu.Root>
				<DropdownMenu.Trigger asChild let:builder>
					<Label>
						<p class="mb-[1px] ml-2 text-xs">Version</p>
						<CustomButton outline={true} color="blue" builders={[builder]}>
							<p class="text-start font-normal text-black">
								{currentVersion.datetime.toLocaleString(undefined, {
									dateStyle: 'long',
									timeStyle: 'short'
								})}
							</p>
							<ChevronDown class="h-4 w-4 text-black" />
						</CustomButton>
					</Label>
				</DropdownMenu.Trigger>
				<DropdownMenu.Content>
					{#each analyzerOutput.versions as version, id}
						<DropdownMenu.Item on:click={() => onVersionChange(version)}>
							<p>
								{version.datetime.toLocaleString(undefined, {
									dateStyle: 'long',
									timeStyle: 'short'
								})}
							</p>
						</DropdownMenu.Item>
					{/each}
				</DropdownMenu.Content>
			</DropdownMenu.Root>
			<div class="flex items-start gap-4">
				<DropdownMenu.Root>
					<DropdownMenu.Trigger asChild let:builder>
						<CustomButton outline={true} color="blue" builders={[builder]}>
							<List class="h-4 w-4" />
							<p>Show/hide columns</p>
						</CustomButton>
					</DropdownMenu.Trigger>
					<DropdownMenu.Content>
						{#each analyzerOutput.fields as field, id}
							<DropdownMenu.CheckboxItem
								bind:checked={showColumnForId[id]}
								on:click={(e) => {
									e.preventDefault();
									showColumnForId[id] = !showColumnForId[id];
								}}
							>
								{field.name}
							</DropdownMenu.CheckboxItem>
						{/each}
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
						{#each analyzerOutput.fields as field, id}
							{#if showColumnForId[id]}
								<DropdownMenu.CheckboxItem
									bind:checked={showFilterForId[id]}
									on:click={(e) => {
										e.preventDefault();
										showFilterForId[id] = !showFilterForId[id];
									}}
								>
									{field.name}
								</DropdownMenu.CheckboxItem>
							{/if}
						{/each}
					</DropdownMenu.Content>
				</DropdownMenu.Root>
			</div>
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

												{#if sortColumn?.id == cell.id && sortColumn?.order == 'asc'}
													<ChevronUp class="ml-2 h-4 w-4" />
												{:else if sortColumn?.id == cell.id && sortColumn?.order == 'desc'}
													<ChevronDown class="ml-2 h-4 w-4" />
												{:else}
													<ChevronsUpDown class="ml-2 h-4 w-4" />
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
