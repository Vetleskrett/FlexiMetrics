<script lang="ts">
	import type { Teacher } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import { Role } from 'src/types/';

	export let userRole: Role;
	export let teachers: Teacher[];
	export let courseId: string = '';
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-4 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="40"
				height="40"
				src="https://img.icons8.com/color/480/teacher.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-2 text-2xl">Teachers</Card.Title>
		</div>

		{#if userRole == Role.Teacher}
			<DropdownMenu.Root>
				<DropdownMenu.Trigger class="ml-auto">
					<EllipsisVertical size="20" />
				</DropdownMenu.Trigger>
				<DropdownMenu.Content>
					<DropdownMenu.Item href={`${courseId}/teachers`}>
						<Pencil class="h-4" />
						<p>Edit teachers</p>
					</DropdownMenu.Item>
				</DropdownMenu.Content>
			</DropdownMenu.Root>
		{/if}
	</Card.Header>
	<Card.Content class="p-0">
		<Table.Root>
			<Table.Header>
				<Table.Row>
					<Table.Head class="h-0 px-6 font-bold text-black">Name</Table.Head>
					<Table.Head class="h-0 px-6 font-bold text-black">Email</Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each teachers as teacher}
					<Table.Row>
						<Table.Cell class="px-6">
							{teacher.name}
						</Table.Cell>
						<Table.Cell class="px-6">
							{teacher.email}
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
