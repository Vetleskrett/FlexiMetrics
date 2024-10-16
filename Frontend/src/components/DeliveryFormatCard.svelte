<script lang="ts">
	import type { DeliveryField } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Ellipsis from 'lucide-svelte/icons/ellipsis';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import Plus from 'lucide-svelte/icons/plus';
	import CustomButton from './CustomButton.svelte';

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

		<CustomButton
			href="/courses/{courseId}/assignments/{assignmentId}/delivery-format"
			color="green"
		>
			<Plus />
			<p>New</p>
		</CustomButton>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			<div class="flex items-center px-6 text-sm font-bold">
				<h1 class="w-1/2">Name</h1>
				<h1>Type</h1>
			</div>
			{#each deliveryFields as deliveryField}
				<Separator class="w-full" />
				<div class="flex items-center px-6 py-4">
					<h1 class="w-1/2">{deliveryField.name}</h1>
					<h1>{deliveryField.type}</h1>

					<DropdownMenu.Root>
						<DropdownMenu.Trigger class="ml-auto">
							<Ellipsis size="20" />
						</DropdownMenu.Trigger>
						<DropdownMenu.Content>
							<DropdownMenu.Item>
								<Trash2 class="h-4" />
								<p>Delete field</p>
							</DropdownMenu.Item>
						</DropdownMenu.Content>
					</DropdownMenu.Root>
				</div>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
