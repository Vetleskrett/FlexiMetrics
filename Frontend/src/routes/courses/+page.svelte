<script lang="ts">
	import type { PageData } from './$types';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Plus from 'lucide-svelte/icons/plus';

	export let data: PageData;

	const courses = [
		{
			id: '1',
			code: 'TDT101',
			name: 'Programmering',
			year: 2024,
			semester: 'Autumn'
		},
		{
			id: '2',
			code: 'TDT102',
			name: 'Databaser',
			year: 2024,
			semester: 'Autumn'
		},
		{
			id: '3',
			code: 'TDT103',
			name: 'Nettverk',
			year: 2024,
			semester: 'Autumn'
		}
	];
</script>

<Card.Root class="m-auto mt-16 w-[700px] overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/knowledge-sharing.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="ml-4 text-3xl">Courses</Card.Title>
		</div>

		<Button
			class="bg-button-green hover:bg-button-green-hover flex h-9 flex-row justify-between gap-2 pl-2 pr-3"
		>
			<Plus />
			<p>New</p>
		</Button>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#each courses as course}
				<Separator class="w-full" />
				<a
					href="/courses/{course.id}"
					class="h-18 flex w-full items-center justify-between p-6 hover:bg-blue-50"
				>
					<h1 class="text-xl">{course.code} - {course.name}</h1>
					<p class="font-semibold text-gray-500">{course.year} {course.semester}</p>
				</a>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
