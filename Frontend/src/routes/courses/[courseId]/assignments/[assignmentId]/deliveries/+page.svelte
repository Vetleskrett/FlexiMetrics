<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import { Input } from '$lib/components/ui/input';
	import ArrowDownToline from 'lucide-svelte/icons/arrow-down-to-line';
	import Save from 'lucide-svelte/icons/save';
	import Undo2 from 'lucide-svelte/icons/undo-2';
	import { Separator } from 'src/lib/components/ui/separator';
	import { course, assignment, deliveryFields, deliveries } from 'src/mockData';
	import CustomButton from 'src/components/CustomButton.svelte';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/courses/{courseId}">{course.code} - {course.name}</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Link href="/courses/{courseId}/assignments/{assignment.id}">
				{assignment.name}
			</Breadcrumb.Link>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>Deliveries</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/pass.png"
				alt="knowledge-sharing"
			/>
			<h1 class="ml-4 text-4xl font-semibold">Deliveries</h1>
		</div>
	</div>

	<Card.Root class="w-[1080px] overflow-hidden p-0">
		<Card.Content class="p-0">
			<Table.Root>
				<Table.Header>
					<Table.Row>
						<Table.Head class="h-0 pt-4 font-bold text-black">Team</Table.Head>
						{#each deliveryFields as field}
							<Table.Head class="h-0 pt-4 font-bold text-black">
								{field.name}
							</Table.Head>
							{#if field.type == 'File'}
								<Table.Head class="h-0 pt-4 font-bold text-black"
									>Upload new [{field.name}]</Table.Head
								>
							{/if}
						{/each}
					</Table.Row>
				</Table.Header>
				<Table.Body>
					{#each deliveries as delivery (delivery.teamId)}
						<Table.Row>
							<Table.Cell>
								{delivery.teamId}
							</Table.Cell>
							{#each deliveryFields as field (field.id)}
								{@const value = delivery.fields.get(field.id)}
								<Table.Cell>
									{#if field.type == 'String'}
										<Input type="text" {value} />
									{:else if field.type == 'Integer'}
										<Input type="number" {value} />
									{:else if field.type == 'Float'}
										<Input type="number" {value} />
									{:else if field.type == 'File'}
										<a href="test" download="test" class="flex items-center text-blue-500">
											<ArrowDownToline size="20" />
											{value}
										</a>
									{:else}
										{value}
									{/if}
								</Table.Cell>
								{#if field.type == 'File'}
									<Table.Cell>
										<Input type="file" id={delivery.teamId + field.id} class="w-48" />
									</Table.Cell>
								{/if}
							{/each}
						</Table.Row>
					{/each}
				</Table.Body>
			</Table.Root>
		</Card.Content>
		<Card.Footer class="flex flex-col items-end p-0">
			<Separator />

			<div class="m-4 flex gap-4">
				<CustomButton color="yellow">
					<Undo2 size="20" />
					<p>Undo changes</p>
				</CustomButton>
				<CustomButton color="green">
					<Save size="20" />
					<p>Save</p>
				</CustomButton>
			</div>
		</Card.Footer>
	</Card.Root>
</div>
