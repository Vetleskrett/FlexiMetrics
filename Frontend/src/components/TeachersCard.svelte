<script lang="ts">
	import type { Teacher } from '../types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';

	export let teachers: Teacher[], courseId: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="32"
				height="32"
				src="https://img.icons8.com/color/480/teacher.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-2 text-2xl">Teachers</Card.Title>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				<EllipsisVertical />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item href="/courses/{courseId}/teachers">
					<Pencil class="h-4" />
					<p>Add/Remove teachers</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#each teachers as teacher}
				<Separator class="w-full" />
				<h1 class="h-18 p-6 text-xl">{teacher.name}</h1>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
