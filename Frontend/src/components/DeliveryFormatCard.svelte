<script lang="ts">
	import type { DeliveryField } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';

	export let deliveryFields: DeliveryField[], assignmentId: string, courseId: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-4 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/metamorphose.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="ml-4 text-3xl">Delivery Format</Card.Title>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				<EllipsisVertical />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item href="/courses/{courseId}/assignments/{assignmentId}/delivery-format">
					<Pencil class="h-4" />
					<p>Edit delivery format</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			<div class="flex items-center px-6 text-sm font-bold">
				<h1 class="w-1/2">Name</h1>
				<h1 class="w-1/2">Type</h1>
			</div>
			{#each deliveryFields as deliveryField}
				<Separator class="w-full" />
				<div class="flex items-center px-6 py-4">
					<h1 class="w-1/2">{deliveryField.name}</h1>
					<h1 class="w-1/2">{deliveryField.type}</h1>
				</div>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
